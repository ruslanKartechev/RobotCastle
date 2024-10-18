using System.Collections;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeInput : MonoBehaviour
    {
        [SerializeField] private bool _refundAllowed;
        private MergeController _mergeController;
        private bool _isActive;
        private Coroutine _positionChecking;
        private BattleMergeUI _battleUI;
        private bool _refundMode;


        public void Init(MergeController mergeController)
        {
            _mergeController = mergeController;
        }

        private void OnDisable()
        {
            SetActive(false);
        }

        public void SetActive(bool active)
        {
            if (_isActive == active)
                return;
            CLog.Log($"Merge input Set active: {active}");
            _isActive = active;
            if (_positionChecking != null)
                StopCoroutine(_positionChecking);
            var ui = ServiceLocator.Get<GameInput>();
            if (active)
            {
                ui.OnDownLongClick += OnDown;
                ui.OnUpMain += OnUp;
                if (_refundAllowed)
                    _battleUI = ServiceLocator.Get<IUIManager>()
                        .Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            }
            else
            {
                ui.OnDownLongClick -= OnDown;
                ui.OnUpMain -= OnUp;
                _mergeController.OnUp(Input.mousePosition);
                if (_refundMode)
                {
                    _refundMode = false;
                    _battleUI.ReturnItemButton.Hide();
                }
            }
        }

        private void OnUp(Vector3 pos)
        {
            if (_refundMode)
            {
                CLog.LogRed($"Refunded money =========");
                ServiceLocator.Get<GameMoney>().levelMoney.AddValue(HeroesConstants.HeroRefundMoney);
                _mergeController.DropAndHideCurrent();
                _battleUI.ReturnItemButton.Hide();
                _refundMode = false;
            }
            else
            {
                _mergeController.OnUp(pos);
            }

            if (ServiceLocator.GetIfContains<BattleCamera>(out var cam))
                cam.SlideBlockers--;
            if (_positionChecking != null)
                StopCoroutine(_positionChecking);
        }

        private void OnDown(Vector3 pos)
        {
            if (ServiceLocator.GetIfContains<BattleCamera>(out var cam))
                cam.SlideBlockers++;
            if (_refundAllowed)
            {
                _positionChecking = StartCoroutine(PositionAndRefundCheck());
            }
            else
            {
                _positionChecking = StartCoroutine(PositionChecking());
            }
            _mergeController.OnDown(pos);
        }


        private IEnumerator PositionChecking()
        {
            while (true)
            {
                _mergeController.OnMove(Input.mousePosition);

                yield return null;
            }
        }

        private IEnumerator PositionAndRefundCheck()
        {
            while (true)
            {
                if (_battleUI.ReturnItemButton.IsAbove() && !_refundMode)
                {
                    var container = ServiceLocator.Get<MergeManager>().Container;
                    var size = container.heroes.Count;
                    if (size > 1)
                    {
                        _refundMode = true;
                        _battleUI.ReturnItemButton.Show();
                    }
                }
                else if (!_battleUI.ReturnItemButton.IsAbove() && _refundMode)
                {
                    _refundMode = false;
                    _battleUI.ReturnItemButton.Hide();
                }
                if(!_refundMode)
                    _mergeController.OnMove(Input.mousePosition);
                yield return null;
            }
        }


    }
}