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
        private int _refundedMoney;

        public void Init(MergeController mergeController)
        {
            _mergeController = mergeController;
        }

   

        public void SetActive(bool active)
        {
            if (_isActive == active)
                return;
            // CLog.Log($"Merge input Set active: {active}");
            _isActive = active;
            if (_positionChecking != null)
                StopCoroutine(_positionChecking);
            var ui = ServiceLocator.Get<GameInput>();
            if (active)
            {
                _mergeController.OnItemPicked += OnDraggingBegan;
                ui.OnDownLongClick += OnDown;
                ui.OnUpMain += OnUp;
                ui.OnDoubleClick += OnDoubleClick;
                if (_refundAllowed)
                    _battleUI = ServiceLocator.Get<IUIManager>()
                        .Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            }
            else
            {
                _mergeController.OnItemPicked -= OnDraggingBegan;
                ui.OnDownLongClick -= OnDown;
                ui.OnUpMain -= OnUp;
                ui.OnDoubleClick -= OnDoubleClick;
                _mergeController.OnUp(Input.mousePosition);
                if (_refundMode)
                {
                    _refundMode = false;
                    _battleUI.ReturnItemButton.Hide();
                }
            }
        }
        
        private void OnDisable() => SetActive(false);

        private void OnDraggingBegan(ItemData itemData)
        {
            if (!_isActive)
                return;
            var lvl = itemData.core.level;
            _refundedMoney = (lvl + 1) * HeroesConstants.HeroRefundMoney;
            _battleUI.ReturnItemButton.SetMoney(_refundedMoney);
        }

        private void OnDoubleClick(Vector3 obj)
        {
            _mergeController.MergeIfPossible(obj);
        }

        private void OnUp(Vector3 pos)
        {
            if (_refundMode)
            {
                var lvl = _mergeController.DropToReturnItem();
                _refundedMoney = (lvl + 1) * HeroesConstants.HeroRefundMoney;
                ServiceLocator.Get<GameMoney>().levelMoney.AddValue(_refundedMoney);
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