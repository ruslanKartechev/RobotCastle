using System.Collections.Generic;
using RobotCastle.Data;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Blacksmith Altar", fileName = "Blacksmith Altar", order = 1)]
    public class BlacksmithAltar : BasicAltar
    {
        [SerializeField] private AltarMp_SmeltUpgrade _mod1;
        [SerializeField] private AltarMp_StartItemSmelt _mod2;
        [SerializeField] private AltarMp_StartItemSmelt _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }


    [System.Serializable]
    public class AltarMp_SmeltUpgrade : AltarMP
    {
        [SerializeField] private List<float> _chances;
        
        public override int SetTier(int tier)
        {
            _tier = tier;

            return _tier;
        }

        public override void Apply()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var chance = _chances[tier];
            CLog.Log($"[AltarMp_SmeltUpgrade] Applying {chance} to level up on summon");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var percent = $"{_chances[tier] * 100}%";
            var d = _description.Replace("<val>", percent);
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
    
    
    
    
     
    [System.Serializable]
    public class AltarMp_StartItemSmelt : AltarMP
    {
        [SerializeField] private List<CoreItemData> _items;


        public override int SetTier(int tier)
        {
            _tier = tier;
            return _tier;
        }

        public override void Apply()
        {
            var itemLevel = _tier == 5 ? 1 : 0;
            var item = new CoreItemData(_items.Random());
            item.level = itemLevel;
            CLog.Log($"[AltarMp_SmeltUpgrade] Will spawn {item.id} lvl_{itemLevel} at the start");
        }

        public override string GetShortDescription()
        {
            var itemLevel = _tier == 5 ? 1 : 0;
            var d = _description.Replace("<val>", itemLevel.ToString());
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;

    }

    
    
    [System.Serializable]
    public class AltarMp_SmeltReroll : AltarMP
    {

        public override int SetTier(int tier)
        {
            _tier = tier;
            return _tier;
        }

        public override void Apply()
        {
            var count = _tier == 5 ? 1 : 0;
            CLog.Log($"[AltarMp_SmeltUpgrade] Will reroll {count} during smelting");
        }

        public override string GetShortDescription()
        {
            var itemLevel = _tier == 5 ? 1 : 0;
            var d = _description.Replace("<val>", itemLevel.ToString());
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;

    }
    
    
    
}