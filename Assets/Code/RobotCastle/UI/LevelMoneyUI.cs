using DG.Tweening;
using RobotCastle.Core;
using SleepDev;
using SleepDev.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class LevelMoneyUI : MoneyToBuyUI
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
        [SerializeField] private float _splashAlphaMax = .5f;
        [SerializeField] private Image _fadeImage;
        [SerializeField] private AddedMoneyAnimator _addedMoneyAnimator;

        protected override void OnUpdated(int newVal, int prevVal)
        {
            base.OnUpdated(newVal, prevVal);
            if (newVal > prevVal)
            {
                AddMoney(newVal, prevVal);
                _fadeImage.DOKill();
                _fadeImage.SetAlpha(_splashAlphaMax);
                _fadeImage.DOFade(0f, _splashTime);
            }
        }

        public void AddMoney(int newVal, int added)
        {
            _addedMoneyAnimator.Animate(added);
        }

    }
}