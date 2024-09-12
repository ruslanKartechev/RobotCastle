using System.Collections;
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
            _isActive = active;
            if(_positionChecking != null)
                StopCoroutine(_positionChecking);
            var ui = ServiceLocator.Get<GameInput>();
            if (active)
            {
                ui.OnDownMain += OnDown;
                ui.OnUpMain += OnUp;
            }
            else
            {
                ui.OnDownMain -= OnDown;
                ui.OnUpMain -= OnUp;
                _mergeController.OnUp(Input.mousePosition);
            }
        }

        private void OnUp(Vector3 pos)
        {
            if(_positionChecking != null)
                StopCoroutine(_positionChecking);
            _mergeController.OnUp(pos);
        }

        private void OnDown(Vector3 pos)
        {
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