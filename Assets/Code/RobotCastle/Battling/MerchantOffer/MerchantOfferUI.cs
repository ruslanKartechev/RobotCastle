using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev.UIElements;
using UnityEngine;

namespace RobotCastle.Battling.MerchantOffer
{
    public class MerchantOfferUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private List<OfferedItemDescriptionUI> _itemDescriptions;
        [SerializeField] private List<MerchantOfferButton> _buttons;
        [SerializeField] private MyButton _completedBtn;
        [SerializeField] private BlackoutFadeScreen _fadeScreen;
        [SerializeField] private MoneyUI _moneyUI;
        private Predicate<MerchantOfferConfig.Goods> _purchaseCallback;
        private Action _endCallback;
        private MerchantOfferConfig.GoodsPreset _preset;
        
        public void Show(MerchantOfferConfig.GoodsPreset preset, List<int> prices, 
            Predicate<MerchantOfferConfig.Goods> purchaseCallback, Action completedCallback)
        {
            _preset = preset;
            _endCallback = completedCallback;
            _purchaseCallback = purchaseCallback;
            var money = ServiceLocator.Get<GameMoney>();
            _moneyUI.Init(money.levelMoney);
            _moneyUI.DoReact(true);
            var count = preset.goods.Count;
            for (var i = 0; i < count; i++)
            {
                var btn = _buttons[i];
                var good = preset.goods[i];
                if (good.forAds)
                {
                    btn.SetAds();  
                }
                else
                {
                    btn.SetNonAds();
                    btn.SetCost(prices[i]);
                }
                btn.activeBtn.SetInteractable(true);
                _itemDescriptions[i].ShowDescription(good.ItemData);
                _itemDescriptions[i].SetIcon(good.ItemData);
            }
            gameObject.SetActive(true);
            _buttons[0].activeBtn.OverrideMainCallback(OnBtn1);
            _buttons[1].activeBtn.OverrideMainCallback(OnBtn2);
            _buttons[2].activeBtn.OverrideMainCallback(OnBtn3);
            _completedBtn.AddMainCallback(Complete);
            _fadeScreen.FadeInWithId(UIConstants.UIMerchantOffer);
        }

        private void TryPurchaseAtIndex(int index)
        {
            var didPurchase = _purchaseCallback.Invoke(_preset.goods[index]);
            if (didPurchase)
            {
                _buttons[index].activeBtn.SetInteractable(false);
            }
        }
        
        private void OnBtn1() => TryPurchaseAtIndex(0);

        private void OnBtn2() => TryPurchaseAtIndex(1);

        private void OnBtn3() => TryPurchaseAtIndex(2);

        private void Complete()
        {
            _fadeScreen.FadeOut();
            _moneyUI.DoReact(false);
            _endCallback?.Invoke();
        }
    }
}