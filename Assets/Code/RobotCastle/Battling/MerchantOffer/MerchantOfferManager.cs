using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;

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
        
        public MerchantOfferConfig config;
        public IGridView playerGrid;
        public IGridSectionsController sectionsController;
        public ITroopSizeManager troops;
        public CastleHealthView healthView;
        public BattleManager battleManager;
        private MerchantOfferData.GoodsPreset _currentPreset;
        private System.Action _callback;
        private int _offerTier;
        
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
            ui.Show(_currentPreset, PurchaseItem, Complete);
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
                if (money < goods.cost)
                    return false;
                money -= goods.cost;
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
                case MergeConstants.TypeItems:
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