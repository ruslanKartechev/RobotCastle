using System.Collections;
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
            var ui = InputBtn.Inst.Btn;
            if (active)
            {
                ui.OnDown += OnDown;
                ui.OnUp += OnUp;
            }
            else
            {
                _mergeController.OnUp(Input.mousePosition);
            }
        }

        private void OnUp()
        {
            if(_positionChecking != null)
                StopCoroutine(_positionChecking);
            _mergeController.OnUp(Input.mousePosition);
        }

        private void OnDown()
        {
            _positionChecking = StartCoroutine(PositionChecking());
            _mergeController.OnDown(Input.mousePosition);

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