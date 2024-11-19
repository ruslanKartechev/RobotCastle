using System;
using RobotCastle.Core;
using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class ShopPurchaseInGameMoney : MonoBehaviour, IShopPurchaseMaker
    {
        [SerializeField] private bool _hardMoney;
        [SerializeField] private int _cost;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private FadeInOutAnimator _successAnimator;
        [SerializeField] private FadeInOutAnimator _failedAnimator;
        

        private void OnEnable()
        {
            _costText.text = _cost.ToString();

            var gm = ServiceLocator.Get<GameMoney>();
            if (_hardMoney)
            {
                UpdateState(gm.globalHardMoney.Val);
                gm.globalHardMoney.OnUpdatedWithContext += OnUpdatedMoney;
            }
            else
            {
                UpdateState(gm.globalMoney.Val);
                gm.globalMoney.OnUpdatedWithContext += OnUpdatedMoney;
            }
        }

        private void OnDisable()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            if (_hardMoney)
                gm.globalHardMoney.OnUpdatedWithContext -= OnUpdatedMoney;
            else
                gm.globalMoney.OnUpdatedWithContext -= OnUpdatedMoney;
        }

        private void OnUpdatedMoney(int newval, int prevval, int context)
        {
            UpdateState(newval);
        }

        private void UpdateState(int money)
        {
            var color = money >= _cost ? Color.white : Color.red;
            _costText.color = color;
        }
        
        public void TryPurchase(Action<EPurchaseResult> callback)
        {
            var gm = ServiceLocator.Get<GameMoney>();
            var mon = _hardMoney ? gm.globalHardMoney : gm.globalMoney;
            
            var owned = mon.Val;
            if (owned < _cost)
            {
                _failedAnimator.OnAndFadeOut();
                callback?.Invoke(EPurchaseResult.NotEnoughMoney);
                return;
            }
            mon.AddValue(-_cost);
            
            _successAnimator.OnAndFadeOut();
            callback?.Invoke(EPurchaseResult.Success);
        }
    }
}