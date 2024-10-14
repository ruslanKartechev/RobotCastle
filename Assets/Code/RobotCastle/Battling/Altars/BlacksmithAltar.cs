using System.Collections.Generic;
using System.Globalization;
using RobotCastle.Battling.SmeltingOffer;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Blacksmith Altar", fileName = "Blacksmith Altar", order = 1)]
    public class BlacksmithAltar : BasicAltar
    {
        [SerializeField] private AltarMp_SmeltUpgrade _mod1;
        [SerializeField] private AltarMp_StartItemSmelt _mod2;
        [SerializeField] private AltarMp_SmeltReroll _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }

    

    [System.Serializable]
    public class AltarMp_SmeltUpgrade : AltarMP, ISmeltModifier
    {
        [SerializeField] private List<float> _chances;

        
        public override void Apply()
        {
            if (_tier < 1) return;
            CLog.Log($"[AltarMp_SmeltUpgrade] Applying ");
            ServiceLocator.Get<SmeltingOfferManager>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var d = _description.Replace("<val>", (_chances[tier] * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnSmeltedWeapon(IItemView view)
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var chance = _chances[tier];
            var r = UnityEngine.Random.Range(0f, 1f);
            r = 0f; // dbg
            if (r < chance)
            {
                var maxLvl = ServiceLocator.Get<MergeMaxLevelCheck>();
                if (maxLvl.CanUpgradeFurther(view.itemData.core))
                {
                    MergeFunctions.AddLevelToItem(view);
                }
            }
        }
    }
    
     
    [System.Serializable]
    public class AltarMp_StartItemSmelt : AltarMP
    {
        [SerializeField] private float _smeltChance;
        [SerializeField] private List<int> _levels;
        [SerializeField] private List<CoreItemData> _items;


        public override void Apply()
        {
            if (_tier < 1) return;
            CLog.Log($"[AltarMp_SmeltUpgrade] Applied. Chance [{_smeltChance*100}%] ");
            var r = UnityEngine.Random.Range(0f, 1f);
            r = 0f; // dbg
            if (r < _smeltChance)
            {
                var tier = _tier >= _levels.Count ? _levels.Count - 1 : _tier;
                var itemLevel = _levels[tier];
                var item = new CoreItemData(_items.Random());
                item.level = itemLevel;
                CLog.Log($"[AltarMp_SmeltUpgrade] Will spawn {item.id} lvl_{itemLevel} at the start");
                ServiceLocator.Get<IPlayerMergeItemsFactory>().SpawnHeroOrItem(new SpawnMergeItemArgs(item));
            }
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _levels.Count ? _levels.Count - 1 : _tier;
            var itemLevel = _levels[tier];
            var d = _description.Replace("<val>", (itemLevel+1).ToString());
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;

    }

    
    
    [System.Serializable]
    public class AltarMp_SmeltReroll : AltarMP
    {
        
        public override void Apply()
        {
            if (_tier < 1) return;
            
            var count = _tier == 6 ? 2 : 1;
            CLog.Log($"[AltarMp_SmeltUpgrade] Adding {count} rerolls to smelting offer");
            ServiceLocator.Get<SmeltingOfferManager>().Rerolls = count;
        }

        public override string GetShortDescription()
        {
            var count = _tier == 6 ? 2 : 1;
            var d = _description.Replace("<val>", count.ToString());
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;

    }
    
    
    
}