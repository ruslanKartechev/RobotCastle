using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling.SmeltingOffer
{
    public class SmeltingOfferManager
    {
        public SmeltingOfferManager(SmeltingConfig config, 
            IGridView gridView, 
            IGridSectionsController sectionsController,
            ITroopSizeManager troopSizeManager,
            Battle battle)
        {
            this.battle = battle;
            this.config = config;
            this.gridView = gridView;
            this.sectionsController = sectionsController;
            this.troopSizeManager = troopSizeManager;
        }

        public Battle battle;
        public SmeltingConfig config;
        public IGridView gridView;
        public ITroopSizeManager troopSizeManager;
        public IGridSectionsController sectionsController;
        private System.Action _callback; 
        private int _offerIndex;

        public static List<CoreItemData> PickThreeItems(List<CoreItemData> itemsOptions)
        {
            const int count = 3;
            var res = new List<CoreItemData>(count);
           
            var options = new List<int>(itemsOptions.Count);
            for (var i = 0; i < itemsOptions.Count; i++)
                options.Add(i);
            for (var i = 0; i < count; i++)
            {
                var index = options.Random();
                options.Remove(index);
                res.Add(itemsOptions[index]);
            }
            return res;
        }
        
        public void MakeNextOffer(System.Action callback)
        {
            if (_offerIndex >= config.smeltingTiers.Count)
            {
                CLog.LogError($"Offer index >= smelting tiers count. Cannot make next offer");
                return;
            }
            var index = _offerIndex;
            _offerIndex++;
            var options = config.smeltingTiers[index].itemsOptions;
            var items = PickThreeItems(options);
            var ui = ServiceLocator.Get<IUIManager>().Show<SmeltingOfferUI>(UIConstants.UISmeltingOffer, () => {});
            ui.ShowOffer(items, OnChoiceConfirmed);
            _callback = callback;
        }

        private void OnChoiceConfirmed(CoreItemData data)
        {
            CLog.Log($"Choice confirmed: {data.AsStr()}");
            switch (data.type)
            {
                case MergeConstants.TypeItems:
                    CLog.Log($"Hero item");
                    var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
                    factory.SpawnHeroOrItem(new SpawnMergeItemArgs(data), gridView, sectionsController, out var view);
                    break;
                case MergeConstants.TypeBonus:
                    CLog.Log($"Bonus");
                    switch (data.id)
                    {
                        case "bonus_troops":
                            CLog.Log($"Adding troops size by 1");
                            troopSizeManager.ExtendBy(1);
                            break;
                        case "bonus_money":
                            CLog.Log($"Adding money +{data.level}");
                            var gm = ServiceLocator.Get<GameMoney>();
                            gm.AddMoney(data.level);
                            break;
                    }
                    break;
            }
            _callback?.Invoke();
        }
    }
}