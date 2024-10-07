using DG.Tweening;
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
        [SerializeField] private float _splashAlphaMax = .5f;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _fadeImage;
        [SerializeField] private AddedMoneyAnimator _addedMoneyAnimator;

        public void DoReact(bool react)
        {
            var gm = ServiceLocator.Get<GameMoney>();
            if (react)
            {
                gm.OnMoneySet += UpdateValue;
                _text.text = $"{gm.levelMoney}";
            }
            else
            {
                gm.OnMoneySet -= UpdateValue;
            }
        }
        
        public void UpdateValue(int newVal, int prevVal)
        {
            _text.text = $"{newVal}";
            _fadeImage.DOKill();
            _fadeImage.SetAlpha(_splashAlphaMax);
            _fadeImage.DOFade(0f, _splashTime);
        }

        public void AddMoney(int newVal, int added)
        {
            _addedMoneyAnimator.Animate(added);
        }

        private void OnEnable()
        {
            if (ServiceLocator.GetIfContains<GameMoney>(out var gm))
            {
                gm.OnMoneySet += UpdateValue;
                gm.OnMoneyAdded += AddMoney;
                _text.text = $"{gm.levelMoney}";
            }
        }

        private void OnDisable()
        {
            if (ServiceLocator.GetIfContains<GameMoney>(out var gm))
            {
                gm.OnMoneySet -= UpdateValue;
                gm.OnMoneyAdded -= AddMoney;        
            }
        }

     
    }
}