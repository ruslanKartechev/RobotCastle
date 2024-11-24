#define TEST_
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling.SmeltingOffer
{
    public class SmeltingOfferManager : IModifiable<ISmeltModifier>
    {
        public SmeltingOfferManager(SmeltingConfig config)
        {
            this.config = config;
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

        public SmeltingConfig config;
        private List<ISmeltModifier> _smeltModifiers = new (10);
        private SmeltingData _currentData;
        private System.Action _callback;
        private int _offerIndex;
        private int _rerolls;


        private List<CoreItemData> PickThreeItems(List<CoreItemData> itemsOptions)
        {
            const int count = 3;
            var res = new List<CoreItemData>(count);
#if !TEST
            var options = new List<int>(itemsOptions.Count);
            for (var i = 0; i < itemsOptions.Count; i++)
                options.Add(i);
            // PRODUCTION
            for (var i = 0; i < count; i++)
            {
                var index = options.Random();
                options.Remove(index);
                res.Add(itemsOptions[index]);
            }
#endif
#if TEST
            // TESTING
            var offerCallIndex = 0;
            itemsOptions = config.smeltingTiers[offerCallIndex].itemsOptions;
            var startInd = 0;
            for (var countInd = 0; countInd < count; countInd++)
            {
                if (itemsOptions.Count <= startInd)
                {
                    startInd = itemsOptions.Count -1; 
                    CLog.LogError($"Error, index overflow");
                }
                res.Add(itemsOptions[startInd]);
                startInd++;
            }
#endif
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
            foreach (var mod in _smeltModifiers)
                data = mod.ModifySmeltItemBeforeApplied(data);
            HeroesManager.AddGameplayRewardOrBonus(data);
            _callback?.Invoke();
        }
        
    }
}