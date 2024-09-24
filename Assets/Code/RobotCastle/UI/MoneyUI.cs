﻿using DG.Tweening;
using RobotCastle.Core;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class MoneyUI : MonoBehaviour
    {
        [System.Serializable]
        private class AddedMoneyAnimator
        {
            [SerializeField] private TextMeshProUGUI _text;
            [SerializeField] private float _animationTime;
            [SerializeField] private float _posStart;
            [SerializeField] private float _posEnd;
            private Sequence _seq;

            public void Animate(int added)
            {
                _text.gameObject.SetActive(true);
                _text.text = $"+{added}";
                var pos = _text.rectTransform.anchoredPosition;
                pos.y = _posStart;
                _text.rectTransform.anchoredPosition = pos;
                _text.alpha = 1f;
                _text.rectTransform.DOAnchorPosX(_posEnd, _animationTime);
                _seq?.Kill();
                var seq = DOTween.Sequence();
                seq.Append(_text.rectTransform.DOAnchorPosX(_posEnd, _animationTime));
                seq.Join(_text.DOFade(0f, _animationTime));
                seq.OnComplete(Off);
                _seq = seq;
            }

            private void Off() => _text.gameObject.SetActive(false);
        }

        [SerializeField] private float _splashTime = .15f;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _fadeImage;
        [SerializeField] private AddedMoneyAnimator _addedMoneyAnimator;

        public void DoReact(bool react)
        {
            var gm = ServiceLocator.Get<GameMoney>();
            if (react)
            {
                gm.OnMoneyUpdated += UpdateValue;
                _text.text = $"{gm.Money}";
            }
            else
            {
                gm.OnMoneyUpdated -= UpdateValue;
            }
        }
        
        public void UpdateValue(int val)
        {
            _text.text = $"{val}";
            _fadeImage.DOKill();
            _fadeImage.SetAlpha(1f);
            _fadeImage.DOFade(0f, _splashTime);
        }

        public void AddMoney(int added)
        {
            _addedMoneyAnimator.Animate(added);
        }

        private void OnEnable()
        {
            if (ServiceLocator.GetIfContains<GameMoney>(out var gm))
            {
                gm.OnMoneyUpdated += UpdateValue;
                gm.OnMoneyAdded += AddMoney;
                _text.text = $"{gm.Money}";
            }
        }

        private void OnDisable()
        {
            if (ServiceLocator.GetIfContains<GameMoney>(out var gm))
            {
                gm.OnMoneyUpdated -= UpdateValue;
                gm.OnMoneyAdded -= AddMoney;        
            }
        }

     
    }
}