using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SleepDev
{
    public class ProperButton : Button
    {
        public event Action OnDown;
        public event Action OnUp;

        private bool _isDown;
        public bool IsDown => _isDown;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _isDown = true;
            OnDown?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _isDown = false;
            OnUp?.Invoke();

        }
    }
}