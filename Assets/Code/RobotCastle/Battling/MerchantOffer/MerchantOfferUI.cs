﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.Battling.MerchantOffer
{
    public delegate bool PurchaseCallback(MerchantOfferData.Goods goods);
    
    public class MerchantOfferUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private List<ItemDescriptionLongUI> _itemDescriptions;
        [SerializeField] private List<MerchantOfferButton> _buttons;
        [SerializeField] private MyButton _completedBtn;
        [SerializeField] private TextMeshProUGUI _moneyText;
        private PurchaseCallback _purchaseCallback;
        private Action _endCallback;
        private MerchantOfferData.GoodsPreset _preset;
        
        public void Show(MerchantOfferData.GoodsPreset preset, PurchaseCallback purchaseCallback, Action completedCallback)
        {
            _preset = preset;
            _endCallback = completedCallback;
            _purchaseCallback = purchaseCallback;
            var money = ServiceLocator.Get<GameMoney>();
            _moneyText.text = money.levelMoney.ToString();
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
                    btn.SetCost(good.cost);
                }
                btn.activeBtn.SetInteractable(true);
                switch (good.ItemData.type)
                {
                    case MergeConstants.TypeItems:
                        _itemDescriptions[i].ShowItem(HeroItemData.GetDataWithDefaultModifiers(good.ItemData));
                        break;
                    case MergeConstants.TypeBonus:
                        _itemDescriptions[i].ShowBonus(good.ItemData);
                        break;
                }
            }
            _buttons[0].activeBtn.OverrideMainCallback(OnBtn1);
            _buttons[1].activeBtn.OverrideMainCallback(OnBtn2);
            _buttons[2].activeBtn.OverrideMainCallback(OnBtn3);
            _completedBtn.AddMainCallback(Complete);
            gameObject.SetActive(true);

        }

        private void TryPurchaseAtIndex(int index)
        {
            var didPurchase = _purchaseCallback.Invoke(_preset.goods[index]);
            if (didPurchase)
            {
                _buttons[index].activeBtn.SetInteractable(false);
                if (_preset.goods[index].forAds == false)
                {
                    var money = ServiceLocator.Get<GameMoney>();
                    _moneyText.text = money.levelMoney.ToString();
                    _moneyText.transform.DOPunchScale(Vector3.one * .2f, .3f);
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
            gameObject.SetActive(false);
            _endCallback?.Invoke();
        }
    }
}