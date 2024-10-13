using System.Collections.Generic;
using System.Globalization;
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
    public class AltarMp_SummonLevelUp : AltarMP
    {
        [SerializeField] private List<float> _chances;
        
        public override void Apply()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var chance = _chances[tier];
            CLog.Log($"[AltarMp_SummonLevelUp] Applying {chance} to level up on summon");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var d = _description.Replace("<val>", (_chances[tier] * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;

    }
    
    
    [System.Serializable]
    public class AltarMp_MergeLevelUp : AltarMP
    {
        [SerializeField] private List<float> _chances;
        
        public override void Apply()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var chance = _chances[tier];
            CLog.Log($"[AltarMp_MergeLevelUp] Applying {chance} to level up on merge");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _chances.Count ? _chances.Count - 1 : _tier;
            var d = _description.Replace("<val>", (_chances[tier] * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }
        
        public override string GetDetailedDescription() => _detailedDescription;

    }
    
    
    [System.Serializable]
    public class AltarMp_BookOfPower : AltarMP
    {
        [SerializeField] private List<int> _levels;
        
        public override int SetTier(int tier)
        {
            _tier = tier;

            return _tier;
        }

        public override void Apply()
        {
            var tier = _tier >= _levels.Count ? _levels.Count - 1 : _tier;
            var bookLvl = _levels[tier];
            CLog.Log($"[AltarMp_BookOfPower] Adding book of power lvl {bookLvl+1}");
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