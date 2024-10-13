using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Giant Altar", fileName = "Giant Altar", order = 3)]
    public class GiantAltar : BasicAltar
    {
        [SerializeField] private AltarMp_AddedDEF _mod1;
        [SerializeField] private AltarMp_StartItemSmelt _mod2;
        [SerializeField] private AltarMp_ReflectDamage _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }


    [System.Serializable]
    public class AltarMp_AddedDEF : AltarMP
    {
        [SerializeField] private List<float> _defence;

        public override int SetTier(int tier)
        {
            _tier = tier;

            return _tier;
        }

        public override void Apply()
        {
            var tier = _tier >= _defence.Count ? _defence.Count - 1 : _tier;
            var val = _defence[tier];
            CLog.Log($"[AltarMp_DamageHPDrain] Tier {tier}, drain percentage: {val}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _defence.Count ? _defence.Count - 1 : _tier;
            var val = _defence[tier];
            var d = _description.Replace("<val>", $"{val}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
    
    
    
    
    [System.Serializable]
    public class AltarMp_HealthAdded : AltarMP
    {
        [SerializeField] private List<float> _defence;

        public override int SetTier(int tier)
        {
            _tier = tier;

            return _tier;
        }

        public override void Apply()
        {
            var tier = _tier >= _defence.Count ? _defence.Count - 1 : _tier;
            var val = _defence[tier];
            CLog.Log($"[AltarMp_DamageHPDrain] Tier {tier}, drain percentage: {val}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _defence.Count ? _defence.Count - 1 : _tier;
            var val = _defence[tier];
            var d = _description.Replace("<val>", $"{val}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
    
    
    
    [System.Serializable]
    public class AltarMp_ReflectDamage : AltarMP
    {
        // halfved for spell damage
        [SerializeField] private List<float> _percentage;

        public override int SetTier(int tier)
        {
            _tier = tier;

            return _tier;
        }

        public override void Apply()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            CLog.Log($"[AltarMp_ReflectDamage] Tier {tier}, reflected percentage: {val}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", $"{val}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
}