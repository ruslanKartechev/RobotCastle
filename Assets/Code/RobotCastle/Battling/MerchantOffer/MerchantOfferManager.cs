﻿using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using SleepDev.Data;
using UnityEngine;

namespace RobotCastle.Battling.MerchantOffer
{
    public class MerchantOfferManager
    {
        public MerchantOfferManager(MerchantOfferConfig config, BattleManager battleManager)
        {
            this.config = config;
            this.battleManager = battleManager;
        }

        public float Sale
        {
            get => _sale;
            set
            {
                _sale = value;
                if (_sale > 1)
                    _sale = 1;
                CLog.Log($"[Merchant Offer] Sale set {_sale * 100}");
            }
        }
        
        public MerchantOfferConfig config;
        public BattleManager battleManager;
        private MerchantOfferConfig.GoodsPreset _currentPreset;
        private System.Action _callback;
        private int _offerTier;
        private float _sale;
        
        public void MakeNextOffer(System.Action callback)
        {
            _callback = callback;
            var tier = _offerTier;
            _offerTier++;
            // _currentPreset = config.optionsPerTier[tier];
            
            var preset = new MerchantOfferConfig.GoodsPreset();
            preset.goods = new List<MerchantOfferConfig.Goods>(3);
            
            var items = new List<string>(){"sword", "staff", "armor", "bow"};
            var bonuses = new List<string>() { "bonus_troops" };
            var health = battleManager.battle.playerHealthPoints;
            if (health < HeroesConstants.PlayerHealthStart)
            {
                bonuses.Add("restore_health");
            }

            for (var i = 0; i < 2; i++)
            {
                var id = items.RemoveRandom();
                preset.goods.Add(new MerchantOfferConfig.Goods()
                {
                    cost = 10,
                    forAds = false,
                    ItemData = new CoreItemData(tier, id, ItemsIds.TypeItem)
                });
            }
            preset.goods.Add(new MerchantOfferConfig.Goods()
            {
                cost = 10,
                forAds = false,
                ItemData = new CoreItemData(1, bonuses.RemoveRandom(), ItemsIds.TypeBonus)
            });
            var randomIndex = UnityEngine.Random.Range(0, preset.goods.Count);
            preset.goods[randomIndex].forAds = true;
            _currentPreset = preset;
            
            var ui = ServiceLocator.Get<IUIManager>().Show<MerchantOfferUI>(UIConstants.UIMerchantOffer, () => { });
            var count = preset.goods.Count;
            var prices = new List<int>(count);
            for(var i = 0; i < count; i++)
            {
                prices.Add(Mathf.RoundToInt(preset.goods[i].cost * (1 - _sale)));
            }
            
            ui.Show(preset, prices, PurchaseItem, Complete);
        }

        private bool PurchaseItem(MerchantOfferConfig.Goods goods)
        {
            CLog.Log($"[{nameof(MerchantOfferManager)}] Trying to purchase: {goods.cost}. ads? {goods.forAds}. {goods.ItemData.AsStr()}");
            if (goods.forAds)
            {
                var (res, msg) = AdsPlayer.Instance.PlayReward((r) =>
                {
                    CLog.Log($"[{nameof(MerchantOfferManager)}] On Ad callback: {r}");
                    if(r) HeroesManager.AddGameplayRewardOrBonus(goods.ItemData);
                }, AdsPlayer.Placement_Merchant);
                CLog.Log($"[{nameof(MerchantOfferManager)}] play rewarded: {res}. {msg}");
                return res;
            }

            var gm = ServiceLocator.Get<GameMoney>();
            var money = gm.levelMoney.Val;
            var cost = Mathf.RoundToInt(goods.cost * (1 - _sale));
            if (money < cost)
                return false;
            money -= cost;
            gm.levelMoney.UpdateWithContext(money, (int)EMoneyChangeContext.AfterPurchase);
            HeroesManager.AddGameplayRewardOrBonus(goods.ItemData);
            return true;
        }


        private void Complete()
        {
            _callback.Invoke();
        }
    }
}