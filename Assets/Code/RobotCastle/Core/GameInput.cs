using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Core
{
    public class GameInput : MonoBehaviour
    {
        [SerializeField] private float _longClockTime = .2f;
        private Vector3 _clickPos;
        public event Action<Vector3> OnDownIgnoreUI;
        public event Action<Vector3> OnUpIgnoreUI;

        
        public event Action<Vector3> OnDownLongClick;
        public event Action<Vector3> OnShortClick;

        public event Action<Vector3> OnDownMain;
        public event Action<Vector3> OnUpMain;
        public event Action<Vector3> OnDownSecond;
        public event Action<Vector3> OnUpSecond;
        public event Action<Vector3> OnSlideMain;
        public event Action<Vector3> OnZoom;

        [SerializeField] private bool _raycastWorldButtons;
        [SerializeField] private LayerMask _worldButtonsLayer;
        private Coroutine _longClickWaiting;
        private IWorldButton _currentWorldButton;
        private InputBtn _inputBtn;
        private bool _mainDown;
        private bool _secondDown;
        private float _mainClickTime;
        
        public bool InputAllowed { get; set; } = true;
        
        public Vector3 MainMousePosition
        {
            get => Input.mousePosition;
        }
        
        public void Init()
        {
            _inputBtn = InputBtn.Create();
            _inputBtn.Btn.OnDown -= OnBtnDown;            
            _inputBtn.Btn.OnDown += OnBtnDown;
            StartCoroutine(Working());
        }

        private void OnBtnDown()
        {
            if (!InputAllowed)
                return;
            if (Input.touchCount > 0)
            {
                foreach (var touch in Input.touches)
                {
                    switch (touch.fingerId)
                    {
                        case 0:
                            if (!_mainDown)
                            {
                                DownMain();
                            }
                            break;
                        case 1:
                            if (!_secondDown)
                            {
                                _secondDown = true;
                                // CLog.Log("[Input] Seconds Touch Down");
                                OnDownSecond?.Invoke(touch.position);
                            }
                            break;
                    }
                }
            }
            else
            {
                DownMain();
            }
        }
        
        private void CheckInputFingersUp()
        {
            if (Input.touchCount > 0)
            {
                foreach (var touch in Input.touches)
                {
                    if (touch.phase is TouchPhase.Canceled or TouchPhase.Ended)
                    {
                        switch (touch.fingerId)
                        {
                            case 0:
                                UpMain();
                                break;
                            case 1:
                                _secondDown = false;
                                // CLog.Log("[Input] Seconds Touch Up");
                                OnUpSecond?.Invoke(touch.position);
                                break;
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    UpMain();
                }
            }
        }
        
        private void DownMain()
        {
            _mainClickTime = Time.time;
            _clickPos = MainMousePosition;
            if(_longClickWaiting != null)
                StopCoroutine(_longClickWaiting);
            _longClickWaiting = StartCoroutine(LongClickChecking());
            _mainDown = true;
            WorldButtons(true, _clickPos);
            OnDownMain?.Invoke(_clickPos);
        }

        private void UpMain()
        {
            _mainDown = false;
            WorldButtons(false, MainMousePosition);
            if(_longClickWaiting != null)
                StopCoroutine(_longClickWaiting);
            if ((Time.time - _mainClickTime) < _longClockTime)
            {
                // CLog.LogRed("[Input] Short click call!");
                OnShortClick?.Invoke(MainMousePosition);
            }
            OnUpMain?.Invoke(MainMousePosition);
        }
        
        private void WorldButtons(bool down, Vector3 screenPos)
        {
            if (!_raycastWorldButtons) return;
            if (down)
            {
                var ray = Camera.main.ScreenPointToRay(screenPos);
                if (Physics.Raycast(ray, out var hit, 250, _worldButtonsLayer))
                {
                    _currentWorldButton = hit.collider.gameObject.GetComponent<IWorldButton>();
                    _currentWorldButton?.OnDown();
                }
            }
            else
            {
                if (_currentWorldButton != null)
                {
                    _currentWorldButton.OnUp();
                    _currentWorldButton = null;
                }   
            }
        }


        private void CheckZoom()
        {
        }

        private void CheckSlide()
        {
            if (_mainDown)
            {
                
            }
        }

        private IEnumerator LongClickChecking()
        {
            var elapsed = 0f;
            while (elapsed < _longClockTime)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            // CLog.LogRed($"[Input] Down long lick after {_longClockTime} sec.");
            OnDownLongClick?.Invoke(_clickPos);
        }

        private IEnumerator Working()
        {
            while (true)
            {
                while (!InputAllowed)
                    yield return null;
                
                if (Input.GetMouseButtonDown(0))
                {
                    OnDownIgnoreUI?.Invoke(MainMousePosition);
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    OnUpIgnoreUI?.Invoke(MainMousePosition);
                }
                
                CheckInputFingersUp();
                CheckSlide();
                CheckZoom();
                
                yield return null;
            }
        }

    }
}