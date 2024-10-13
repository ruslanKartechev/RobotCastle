using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Mage Altar", fileName = "Mage Altar", order = 4)]
    public class MageAltar : BasicAltar
    {
        [SerializeField] private AltarMp_AddedDEF _mod1;
        [SerializeField] private AltarMp_FinalSp _mod2;
        [SerializeField] private AltarMp_MpUponKill _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }


    [System.Serializable]
    public class AltarMp_InitialMp : AltarMP
    {
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
            CLog.Log($"[AltarMp_DamageHPDrain] Tier {tier}, initial mp percentage: {val}");
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
    
    
    [System.Serializable]
    public class AltarMp_FinalSp : AltarMP
    {
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
            CLog.Log($"[AltarMp_FinalSp] Tier {tier}, final spell power: {val}");
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
    
    
       
    [System.Serializable]
    public class AltarMp_MpUponKill : AltarMP
    {
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
            CLog.Log($"[AltarMp_MpUponKill] Tier {tier}, mp upon kill percentage: {val}");
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