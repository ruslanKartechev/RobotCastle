using System;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class LayoutSwitcher : MonoBehaviour
    {
        [SerializeField] private List<LayoutBase> _layouts;
        private int _index = 0;
        
        public void SetLayout(int index, Action onDone, bool animated = true)
        {
            var layout = _layouts[index];
            layout.SetLayout(onDone, animated);
        }

        public void SetNextLayout()
        {
            if (_layouts.Count == 0)
                return;
            _index++;
            if (_index >= _layouts.Count)
                _index = 0;
            _layouts[_index].SetLayout(() => {}, false);
        }
        
        public void SetPrevLayout()
        {
            if (_layouts.Count == 0)
                return;
            _index--;
            if (_index < 0)
                _index = _layouts.Count - 1;
            _layouts[_index].SetLayout(() => {}, false);
        }
    }
}