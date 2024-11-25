using System.Collections.Generic;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ChapterSelectButton : MyButton
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Color _colorPassive;
        [SerializeField] private Color _colorActive;
        [SerializeField] private ScalePulser _animator;
        [SerializeField] private List<GameObject> _activeGos;
        private bool _state = false;
        
        public void SetState(bool active)
        {
            if (_state == active)
                return;
            _state = active;
            if (active)
            {
                _icon.color = _colorActive;
                _animator.Begin();
                foreach (var go in _activeGos)
                    go.SetActive(true);
            }
            else
            {
                _icon.color = _colorPassive;
                _animator.Reset();
                foreach (var go in _activeGos)
                    go.SetActive(false);
            }
        }


    }
}