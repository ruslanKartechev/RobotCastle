using System;
using System.Collections.Generic;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    public class DevCheatUI : MonoBehaviour
    {
        private static DevCheatUI _inst;
        public static DevCheatUI Inst => _inst;

        [SerializeField] private List<TabData> _tabs;
        [SerializeField] private MyButton _exitBtn;
        [SerializeField] private MyButton _openBtn;
        [SerializeField] private GameObject _goOpen;
        [SerializeField] private GameObject _goClosed;
        [SerializeField] private float _doubleClickTime = .15f;
        private Tab _current;
        private int _clicks;
        private float _clickTime;

        private void Awake()
        {
            if (_inst == null)
            {
                _inst = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        [ContextMenu("Open")]
        public void Open()
        {
            if (_clicks <= 0)
            {
                _clicks = 1;
                _clickTime = Time.deltaTime;
            }
            else if (_clicks >= 1)
            {
                if (Time.deltaTime - _clickTime <= _doubleClickTime)
                {
                    _clicks = 0;
                    gameObject.SetActive(true);
                    CloseCurrent();
                    _goOpen.SetActive(true);
                    _goClosed.SetActive(false);
                    OpenMainMenu();
                    Time.timeScale = 0f;
                }
                else
                    _clicks = 0;
            }
        }

        [ContextMenu("Exit")]
        public void Exit()
        {
            _goOpen.SetActive(false);
            _goClosed.SetActive(true);
            Time.timeScale = 1f;
        }

        private void CloseCurrent()
        {
            if (_current != null)
            {
                _current.Close();
                _current = null;
            }
        }

        private void OpenMainMenu()
        {
            _current = _tabs[0].tab;
            _current.Show(OpenMainMenu);
        }

        private void Start()
        {
            _exitBtn.AddMainCallback(Exit);
            _openBtn.AddMainCallback(Open);
            foreach (var tt in _tabs)
            {
                tt.Init(CloseCurrent, OpenMainMenu);
            }
        }

        [System.Serializable]
        public class TabData
        {
            public Tab tab;
            public MyButton btn;
            private Action _callbackOpen;
            private Action _callbackClosed;

            public void Init(Action callbackOpen, Action callbackClosed)
            {
                _callbackClosed = callbackClosed;
                _callbackOpen = callbackOpen;
                if (btn == null)
                    return;
                btn.AddMainCallback(Open);
            }

            private void Open()
            {
                _callbackOpen?.Invoke();
                tab.Show(_callbackClosed);
            }
        }
    }
}