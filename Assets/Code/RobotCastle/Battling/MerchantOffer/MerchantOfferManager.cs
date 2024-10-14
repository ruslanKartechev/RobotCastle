using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.MerchantOffer
{
    public class MerchantOfferManager
    {
        public MerchantOfferManager(MerchantOfferConfig config, IGridView playerGrid,
            IGridSectionsController sectionsController, ITroopSizeManager troops, 
            BattleManager battleManager, CastleHealthView healthView)
        {
            this.config = config;
            this.sectionsController = sectionsController;
            this.troops = troops;
            this.battleManager = battleManager;
            this.healthView = healthView;
            this.playerGrid = playerGrid;
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
        public IGridView playerGrid;
        public IGridSectionsController sectionsController;
        public ITroopSizeManager troops;
        public CastleHealthView healthView;
        public BattleManager battleManager;
        private MerchantOfferData.GoodsPreset _currentPreset;
        private System.Action _callback;
        private int _offerTier;
        private float _sale;
        
        public void MakeNextOffer(System.Action callback)
        {
            if (_offerTier >= config.optionsPerTier.Count)
            {
                CLog.LogError($"[{nameof(MerchantOfferManager)}] _index >= config.optionsPerTier.Count");
                return;
            }
            _callback = callback;
            var tier = _offerTier;
            _offerTier++;
            _currentPreset = config.optionsPerTier[tier].GetRandomPreset();
            var ui = ServiceLocator.Get<IUIManager>().Show<MerchantOfferUI>(UIConstants.UIMerchantOffer, () => { });
            var count = _currentPreset.goods.Count;
            var prices = new List<int>(count);
            for(var i = 0; i < count; i++)
            {
                prices.Add(Mathf.RoundToInt(_currentPreset.goods[i].cost * (1 - _sale)));
            }
            
            ui.Show(_currentPreset, prices, PurchaseItem, Complete);
        }

        private bool PurchaseItem(MerchantOfferData.Goods goods)
        {
            CLog.Log($"[{nameof(MerchantOfferManager)}] Trying to purchase: {goods.cost}. ads? {goods.forAds}. {goods.ItemData.AsStr()}");
            if (goods.forAds)
            {
                CLog.Log($"Trying to show reward ads to get an item");
                var (res, msg) = AdsPlayer.Instance.PlayReward((res) =>
                {
                    if(res)
                        GrantGoods(goods); 
                },"merchant_offer");
                return res;
            }
            else
            {
                var gm = ServiceLocator.Get<GameMoney>();
                var money = gm.levelMoney;
                var cost = Mathf.RoundToInt(goods.cost * (1 - _sale));
                if (money < cost)
                    return false;
                money -= cost;
                gm.levelMoney = money;
                GrantGoods(goods);
                return true;
            }
        }

        private void GrantGoods(MerchantOfferData.Goods goods)
        {
            var data = goods.ItemData;
            switch (data.type)
            {
                case MergeConstants.TypeWeapons:
                    var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
                    factory.SpawnHeroOrItem(new SpawnMergeItemArgs(data), playerGrid, sectionsController, out var item);
                    break;
                case MergeConstants.TypeBonus:
                    switch (data.id)
                    {
                        case "bonus_troops":
                            CLog.Log($"Adding troops size by 1");
                            troops.ExtendBy(1);
                            break;
                        case "bonus_money":
                            CLog.Log($"Adding money +{data.level}");
                            var gm = ServiceLocator.Get<GameMoney>();
                            gm.AddMoney(data.level);
                            break;
                    }
                    break;
            }            
        }        

        private void Complete()
        {
            _callback.Invoke();
        }
    }
}