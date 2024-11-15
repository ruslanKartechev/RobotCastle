﻿using System;
using System.Collections.Generic;
using RobotCastle.UI;
using SleepDev;
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
            PrintCurrentIndex();
        }

        public void Stop() => _isWorking = false;

        public void SetTitle(string title) => _titleText.text = title;

        public void Show() => _fadeInOutAnimator.FadeIn();

        public void Hide() => _fadeInOutAnimator.FadeOut();
        
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
        private List<string> _messages;
        private int _index;
        private bool _isWorking;
        private Action _callback;
        private string _currentFullMsg;

        private void Start()
        {
            _skipBtn.AddMainCallback(Skip);
        }

        private void Skip()
        {
            CLog.LogRed("=== Btn skip text");
            if(!_isWorking) return;
            _index++;
            if (_index >= _messages.Count)
            {
                _textFiller.StopFilling();
                _textFiller.SetText(_messages[^1]);
                _isWorking = false;
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
            _textFiller.AnimateText(_currentFullMsg, Skip);
        }

    }
}