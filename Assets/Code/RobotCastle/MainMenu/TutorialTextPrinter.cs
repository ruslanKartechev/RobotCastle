using System;
using System.Collections.Generic;
using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class TutorialTextPrinter : MonoBehaviour
    {
        public void ShowMessages(List<string> messages)
        {
            _isWorking = true;
            _messages = messages;
            _index = 0;
            foreach (var go in _offWhenDone)
                go.SetActive(true);
            PrintCurrentIndex();
        }

        public void Stop() => _isWorking = false;

        public void SetTitle(string title) => _titleText.text = title;

        public void Show()
        {
            gameObject.SetActive(true);
            _fadeInOutAnimator.FadeIn();
        }

        public void Hide()
        {
            _fadeInOutAnimator.FadeOut();
        }
        
        public void On() => _fadeInOutAnimator.On();
        
        public void Off() => _fadeInOutAnimator.Off();

        public void HideNow() => _fadeInOutAnimator.Off();

        public Action Callback
        {
            get => _callback;
            set => _callback = value;
        }

        public bool IsAdditive
        {
            get => _isAdditive;
            set => _isAdditive = value;
        }

        [SerializeField] private bool _isAdditive;
        [SerializeField] private FadeInOutAnimator _fadeInOutAnimator;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private MyButton _skipBtn;
        [SerializeField] private TextFiller _textFiller;
        [SerializeField] private List<GameObject> _offWhenDone;
        private List<string> _messages;
        private int _index;
        private bool _isWorking;
        private Action _callback;
        private string _currentFullMsg;

        private void Start()
        {
            _skipBtn.AddMainCallback(OnBtnSkip);
            _skipBtn.SetInteractable(true);
        }

        private void OnBtnSkip()
        {
            Next();            
        }
        
        private void Next()
        {
            if(!_isWorking) return;
            _index++;
            if (_index >= _messages.Count)
            {
                _textFiller.StopFilling();
                _textFiller.SetText(_messages[^1]);
                _isWorking = false;
                foreach (var go in _offWhenDone)
                    go.SetActive(false);
                _callback?.Invoke();
                return;
            }
            PrintCurrentIndex();
        }
        
        private void PrintCurrentIndex()
        {
            if (_isAdditive)
                _currentFullMsg += _messages[_index];
            else
                _currentFullMsg = _messages[_index];
            _textFiller.StopFilling();
            _textFiller.AnimateText(_currentFullMsg, Next);
        }

    }
}