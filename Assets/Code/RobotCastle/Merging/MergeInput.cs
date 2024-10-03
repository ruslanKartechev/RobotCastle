using System.Collections;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class MergeInput : MonoBehaviour
    {
        private MergeController _mergeController;
        private bool _isActive;
        private Coroutine _positionChecking;
        
        
        public void Init(MergeController mergeController)
        {
            _mergeController = mergeController;
        }

        public void SetActive(bool active)
        {
            if (_isActive == active)
                return;
            CLog.Log($"Merge input Set active: {active}");
            _isActive = active;
            if(_positionChecking != null)
                StopCoroutine(_positionChecking);
            var ui = ServiceLocator.Get<GameInput>();
            if (active)
            {
                ui.OnDownLongClick += OnDown;
                ui.OnUpMain += OnUp;
            }
            else
            {
                ui.OnDownLongClick -= OnDown;
                ui.OnUpMain -= OnUp;
                _mergeController.OnUp(Input.mousePosition);
            }
        }

        private void OnUp(Vector3 pos)
        {
            if (ServiceLocator.GetIfContains<BattleCamera>(out var cam))
                cam.SlideBlockers--;
            if(_positionChecking != null)
                StopCoroutine(_positionChecking);
            _mergeController.OnUp(pos);
        }

        private void OnDown(Vector3 pos)
        {
            if (ServiceLocator.GetIfContains<BattleCamera>(out var cam))
                cam.SlideBlockers++;
            _positionChecking = StartCoroutine(PositionChecking());
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


    }
}