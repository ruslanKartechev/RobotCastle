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
        
        public event Action<Vector3> OnDownMain;
        public event Action<Vector3> OnUpMain;
        public event Action<Vector3> OnDownSecond;
        public event Action<Vector3> OnUpSecond;
        public event Action<Vector3> OnSlideMain;
        public event Action<Vector3> OnZoom;

        [SerializeField] private bool _raycastWorldButtons;
        [SerializeField] private LayerMask _worldButtonsLayer;
        private IWorldButton _currentWorldButton;
        private InputBtn _inputBtn;
        private bool _mainDown;
        private bool _secondDown;
        
        public bool InputAllowed { get; set; } = true;
        
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
                                _mainDown = true;
                                // CLog.Log("[Input] Main Touch Down");
                                WorldButtons(true, touch.position);
                                OnDownMain?.Invoke(touch.position);
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
                _mainDown = true;
                WorldButtons(true, Input.mousePosition);
                OnDownMain?.Invoke(Input.mousePosition);
            }
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
                                _mainDown = false;
                                // CLog.Log("[Input] Main Touch Up");
                                WorldButtons(false, touch.position);
                                OnUpMain?.Invoke(touch.position);
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
                    _mainDown = false;
                    WorldButtons(false, Input.mousePosition);
                    OnUpMain?.Invoke(Input.mousePosition);
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
        
        private IEnumerator Working()
        {
            while (true)
            {
                while (!InputAllowed)
                    yield return null;
                if (Input.GetMouseButtonDown(0))
                {
                    OnDownIgnoreUI?.Invoke(Input.mousePosition);
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    OnUpIgnoreUI?.Invoke(Input.mousePosition);
                }
                
                CheckInputFingersUp();
                CheckSlide();
                CheckZoom();
                
                yield return null;
            }
        }

    }
}