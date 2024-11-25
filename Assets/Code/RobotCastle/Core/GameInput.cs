using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Core
{
    public class GameInput : MonoBehaviour
    {
        public event Action<Vector3> OnDownIgnoreUI;
        public event Action<Vector3> OnUpIgnoreUI;
        public event Action<Vector3> OnDownLongIgnoreUIClick;

        public event Action<Vector3> OnDoubleClick;
        public event Action<Vector3> OnDownLongClick;
        public event Action<Vector3> OnShortClick;

        public event Action<Vector3> OnDownMain;
        public event Action<Vector3> OnUpMain;
        public event Action<Vector3> OnDownSecond;
        public event Action<Vector3> OnUpSecond;
        public event Action<Vector3> OnSlideMain;
        public event Action<Vector3> OnZoom;

        public void ResetEvents()
        {
            OnDownIgnoreUI = null;
            OnUpIgnoreUI = null;
            OnDownLongIgnoreUIClick = null;
            OnDoubleClick = null;
            OnDownLongClick = null;
            OnShortClick = null;
            OnShortClick = null;
            OnDownMain = null;
            OnUpMain = null;
            OnDownSecond = null;
            OnUpSecond = null;
            OnSlideMain = null;
            OnZoom = null;
        }

        [SerializeField] private float _longClockTime = .2f;
        [SerializeField] private bool _raycastWorldButtons;
        [SerializeField] private LayerMask _worldButtonsLayer;
        private Vector3 _clickPos;
        private Coroutine _longClickWaiting;
        private Coroutine _longClickWaitingIgnoreUI;
        
        private IWorldButton _currentWorldButton;
        private InputBtn _inputBtn;
        private bool _mainDown;
        private bool _secondDown;
        private float _mainClickTime;
        private bool _didSlide;
        private Vector3 _slide;
        private float _slideMinD2 = 300f * 300f;
        private float _prevClickTime;
        private int _clicksCount;
        private bool _inputAllowed = true;
        
        public bool InputAllowed
        {
            get => _inputAllowed;
            set
            {
                _inputAllowed = value;
                if (!value)
                {
                    _clicksCount = 0;
                    _mainDown = _secondDown = _didSlide = false;
                }
            } 
        } 
        
        public Vector3 MainMousePosition
        {
            get => Input.mousePosition;
        }
        
        public void Init()
        {
            _inputBtn = InputBtn.Create();
            _inputBtn.Btn.OnDown -= OnUIBtnDown;            
            _inputBtn.Btn.OnDown += OnUIBtnDown;
            StartCoroutine(Working());
        }

        private void OnUIBtnDown()
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

        private void CheckDoubleUIClick()
        {
            // if (_prevClickTime == 0)
            // {
            //     _prevClickTime = _mainClickTime;
            //     return;
            // }
            if (_mainClickTime - _prevClickTime < _longClockTime)
                OnDoubleClick?.Invoke(MainMousePosition);
            _prevClickTime = _mainClickTime;
        }
        
        private void DownMain()
        {
            _mainClickTime = Time.time;
            CheckDoubleUIClick();
            _clickPos = MainMousePosition;
            _mainDown = true;
            WorldButtons(true, _clickPos);
            OnDownMain?.Invoke(_clickPos);
            if(_longClickWaiting != null)
                StopCoroutine(_longClickWaiting);
            _longClickWaiting = StartCoroutine(LongClickChecking());
        }

  
        private void UpMain()
        {
            ResetSlide();
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
        { }

        private void ResetSlide()
        {
            _didSlide = false;
            _slide = Vector3.zero;
        }
        
        private IEnumerator LongClickChecking()
        {
            var elapsed = 0f;
            while (elapsed < _longClockTime)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            // CLog.LogRed($"[Input] Down long lick after {_longClockTime} sec.");
            OnDownLongClick?.Invoke(_clickPos);
        }
        
        private IEnumerator LongClickIgnoreUIChecking()
        {
            var elapsed = 0f;
            while (elapsed < _longClockTime)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            OnDownLongIgnoreUIClick?.Invoke(Input.mousePosition);
        }
        
        private IEnumerator Working()
        {
            var oldMainPos = Input.mousePosition;
            var didWait = false;
            while (true)
            {
                while (!InputAllowed)
                {
                    didWait = true;
                    yield return null;
                }
                if (didWait)
                {
                    oldMainPos = Input.mousePosition;
                    didWait = false;
                }
                
                var mainPos = MainMousePosition;
                if (Input.GetMouseButtonDown(0))
                {
                    OnDownIgnoreUI?.Invoke(MainMousePosition);
                    if(_longClickWaitingIgnoreUI != null)
                        StopCoroutine(_longClickWaitingIgnoreUI);
                    _longClickWaitingIgnoreUI = StartCoroutine(LongClickIgnoreUIChecking());
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    if(_longClickWaitingIgnoreUI != null)
                        StopCoroutine(_longClickWaitingIgnoreUI);
                    OnUpIgnoreUI?.Invoke(MainMousePosition);
                }
                else if (_mainDown && !_didSlide)
                {
                    var delta = mainPos - oldMainPos;
                    _slide += delta;
                    if (_slide.sqrMagnitude >= _slideMinD2)
                    {
                        _didSlide = true;
                        OnSlideMain?.Invoke(_slide);
                    }
                }
                CheckInputFingersUp();
                CheckZoom();
                oldMainPos = mainPos;
                yield return null;
            }
        }

    }
}