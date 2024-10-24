using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev.UIElements;
using UnityEngine;

namespace RobotCastle.Battling.MerchantOffer
{
    public delegate bool PurchaseCallback(MerchantOfferData.Goods goods);
    
    public class MerchantOfferUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private List<ItemDescriptionLongUI> _itemDescriptions;
        [SerializeField] private List<MerchantOfferButton> _buttons;
        [SerializeField] private MyButton _completedBtn;
        [SerializeField] private BlackoutFadeScreen _fadeScreen;
        [SerializeField] private MoneyUI _moneyUI;
        private PurchaseCallback _purchaseCallback;
        private Action _endCallback;
        private MerchantOfferData.GoodsPreset _preset;
        
        public void Show(MerchantOfferData.GoodsPreset preset, List<int> prices, PurchaseCallback purchaseCallback, Action completedCallback)
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
                switch (good.ItemData.type)
                {
                    case MergeConstants.TypeWeapons:
                        _itemDescriptions[i].ShowItem(HeroWeaponData.GetDataWithDefaultModifiers(good.ItemData));
                        break;
                    case MergeConstants.TypeBonus:
                        _itemDescriptions[i].ShowBonus(good.ItemData);
                        break;
                }
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
                if (_preset.goods[index].forAds == false)
                {
                }
            }
        }
        
        private void OnBtn1()
        {
            TryPurchaseAtIndex(0);
        }

        private void OnBtn2()
        {
            TryPurchaseAtIndex(1);
        }

        private void OnBtn3()
        {
            TryPurchaseAtIndex(2);
        }

        private void Complete()
        {
            _fadeScreen.FadeOut();
            _moneyUI.DoReact(false);
            _endCallback?.Invoke();
        }
    }
}