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
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _fadeImage;

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
            _fadeImage.DOFade(0f, .2f);
        }

        private void OnEnable()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            gm.OnMoneyUpdated += UpdateValue;
            _text.text = $"{gm.Money}";
        }

        private void OnDisable()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            gm.OnMoneyUpdated -= UpdateValue;
        }

     
    }
}