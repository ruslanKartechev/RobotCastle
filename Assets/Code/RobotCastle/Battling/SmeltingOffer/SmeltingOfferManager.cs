using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling.SmeltingOffer
{
    public class SmeltingOfferManager : IModifiable<ISmeltModifier>
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

        public int Rerolls
        {
            get => _rerolls;
            set
            {
                _rerolls = value;
                if (_rerolls < 0)
                    _rerolls = 0;
                CLog.Log($"[Smelting Offer] Rerolls set {_rerolls}");
            }
        }

        public Battle battle;
        public SmeltingConfig config;
        public IGridView gridView;
        public ITroopSizeManager troopSizeManager;
        public IGridSectionsController sectionsController;
        private List<ISmeltModifier> _smeltModifiers = new (10);
        private SmeltingData _currentData;
        private System.Action _callback;
        private int _offerIndex;
        private int _rerolls;

        
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
            _currentData = config.smeltingTiers[index]; 
            var options = _currentData.itemsOptions;
            var items = PickThreeItems(options);
            var ui = ServiceLocator.Get<IUIManager>().Show<SmeltingOfferUI>(UIConstants.UISmeltingOffer, () => { });
            ui.ShowOffer(items, this);
            _callback = callback;
        }

        public List<CoreItemData> RerollItems()
        {
            if (_rerolls == 0)
                return null;
            _rerolls--;
            if (_rerolls < 0)
                _rerolls = 0;
            var items = PickThreeItems(_currentData.itemsOptions);
            return items;
        }

        public void AddModifier(ISmeltModifier mod)
        {
            _smeltModifiers.Add(mod);
        }

        public void RemoveModifier(ISmeltModifier mod)
        {
            _smeltModifiers.Remove(mod);
        }

        public void ClearAllModifiers()
        {
            _smeltModifiers.Clear();
        }
        
        public void OnChoiceConfirmed(CoreItemData data)
        {
            // CLog.Log($"Choice confirmed: {data.AsStr()}");
            var originalLevel = data.level;
            foreach (var mod in _smeltModifiers)
                data = mod.ModifySmeltItemBeforeApplied(data);
            switch (data.type)
            {
                case MergeConstants.TypeWeapons:
                    var factory = ServiceLocator.Get<IPlayerMergeItemsFactory>();
                    var view = factory.SpawnHeroOrItem(new SpawnMergeItemArgs(data));
                    if(data.level > originalLevel)
                        MergeFunctions.PlayMergeFX(view);
                    break;
                case MergeConstants.TypeBonus:
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