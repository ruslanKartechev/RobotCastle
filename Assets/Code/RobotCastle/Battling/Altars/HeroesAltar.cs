using System.Collections.Generic;
using System.Globalization;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Heroes Altar", fileName = "Heroes Altar", order = 0)]
    public class HeroesAltar : BasicAltar
    {
        [SerializeField] private AltarMp_SummonLevelUp _mod1;
        [SerializeField] private AltarMp_MergeLevelUp _mod2;
        [SerializeField] private AltarMp_BookOfPower _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }
    
    
    [System.Serializable]
    public class AltarMp_SummonLevelUp : AltarMP, IPlayerItemSpawnModifier
    {
        [SerializeField] private List<float> _chances;
        
        public override void Apply()
        {
            if (_tier < 1)
                return;
            CLog.Log($"[AltarMp_SummonLevelUp] Applied");
            ServiceLocator.Get<IPlayerMergeItemsFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var d = _description.Replace("<val>", (_chances[tier] * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;

        public void OnNewItemSpawned(IItemView view)
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var chance = _chances[tier];
            CLog.LogGreen($"[AltarMp_SummonLevelUp] Chance: {chance * 100} ");
            var r = UnityEngine.Random.Range(0f, 1f);
            // r = 0f; // dbg
            if (r < chance)
            {
                var maxLvl = ServiceLocator.Get<IMergeMaxLevelCheck>();
                if(maxLvl.CanUpgradeFurther(view.itemData.core))
                {
                    MergeFunctions.AddLevelWithFX(view);
                }
            }
        }
    }
    
    
    [System.Serializable]
    public class AltarMp_MergeLevelUp : AltarMP, IMergeModifier
    {
        [SerializeField] private List<float> _chances;
        
        public override void Apply()
        {
            if (_tier < 1)
                return;
            CLog.Log($"[AltarMp_MergeLevelUp] Applied");
            ServiceLocator.Get<IMergeProcessor>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var d = _description.Replace("<val>", (_chances[tier] * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnMergedOneIntoAnother(IItemView itemHidden, IItemView itemMergedInto)
        {
            CLog.LogRed("!! On merged received");
            var view = itemMergedInto;
            if (view.itemData.core.type == MergeConstants.TypeHeroes)
            {
                var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
                var chance = _chances[tier];
                CLog.LogGreen($"[AltarMp_MergeLevelUp] Chance: {chance * 100} ");

                var r = UnityEngine.Random.Range(0f, 1f);
                // r = 0f; // dbg
                if (r < chance)
                {
                    var maxLvl = ServiceLocator.Get<IMergeMaxLevelCheck>();
                    if(maxLvl.CanUpgradeFurther(view.itemData.core))
                    {
                        MergeFunctions.AddLevelToItem(view);
                    }
                }
            }
        }

        public void OnNewItemSpawnDuringMerge(IItemView newItem, IItemView item1, IItemView item2) // ignored, heroes not spawned like that
        { }
    }
    
    
    [System.Serializable]
    public class AltarMp_BookOfPower : AltarMP
    {
        [SerializeField] private List<int> _levels;
        
        public override void Apply()
        {
            if (_tier < 1)
                return;
            var tier = _tier >= _levels.Count ? _levels.Count - 1 : _tier;
            var bookLvl = _levels[tier];
            CLog.Log($"[AltarMp_BookOfPower] [Apply] Adding book of power lvl {bookLvl+1}");
            ServiceLocator.Get<IBattleStartData>().AddStartItem(new CoreItemData(bookLvl, MergeConstants.UpgradeBookId, MergeConstants.TypeWeapons));
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _levels.Count ? _levels.Count - 1 : _tier;
            var bookLvl = _levels[tier];
            var d = _description.Replace("<val>", (bookLvl+1).ToString());
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;
        
        
    }
}