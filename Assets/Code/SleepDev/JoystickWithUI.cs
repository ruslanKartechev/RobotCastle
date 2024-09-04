using System;
using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class JoystickWithUI : JoystickController
    {
        [SerializeField] private float _controlsMaxRad = 100;
        
        private IJoystickUI _uiStick;
        private ProperButton _inputBtn;
        
        public Action OnDownCallback { get; set; }
        public Action OnUpCallback { get; set; }
        public Action<Vector3> OnXZMoveCallback { get; set; }

        public void Init(IJoystickUI ui, ProperButton inputBtn)
        {
            _inputBtn = inputBtn;
            _uiStick = ui;
            MaxRad = _controlsMaxRad;
        }

        public void Activate()
        {
            Stop();
            _inputBtn.OnDown += OnDown;
            // _inputBtn.OnUp += OnUp;

        }

        public override void Stop()
        {
            base.Stop();
            _inputBtn.OnDown -= OnDown;
            // _inputBtn.OnUp -= OnUp;
            _uiStick.Hide();
        }
        
        private void OnDown()
        {
            base.Stop();
            OnDownCallback?.Invoke();
            Reset();
            _uiStick.Show();
            _uiStick.SetPosition(Input.mousePosition);
            _uiStick.SetJoystickLocal(_position);
            _working = StartCoroutine(FullInputChecking());
        }

        private void OnUp()
        {
            base.Stop();
            _uiStick.Hide();
            OnUpCallback?.Invoke();
        }

        protected IEnumerator FullInputChecking()
        {
            var pos = Input.mousePosition;
            var isDown = false;
            while (true)
            {
                if (Input.GetMouseButton(0))
                {
                    var nPos = Input.mousePosition;
                    Vector2 delta = nPos - pos;
                    delta *= Sensitivity;
                    Move(delta);
                    pos = nPos;
                    _uiStick.SetJoystickLocal(_position);
                    OnXZMoveCallback?.Invoke(GetScaledXZPlane());
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    OnUp();
                }
                yield return null;
            }
        }
        
    }
}