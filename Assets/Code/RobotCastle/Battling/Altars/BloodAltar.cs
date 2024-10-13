using System.Collections.Generic;
using System.Globalization;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Blood Altar", fileName = "Blood Altar", order = 2)]
    public class BloodAltar : BasicAltar
    {
        [SerializeField] private AltarMp_DamageHPDrain _mod1;
        [SerializeField] private AltarMp_AttackIncrease _mod2;
        [SerializeField] private AltarMp_MightyBlock _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }


    [System.Serializable]
    public class AltarMp_DamageHPDrain : AltarMP
    {
        [SerializeField] private List<float> _percentage;

        public override void Apply()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var drain = _percentage[tier];
            CLog.Log($"[AltarMp_DamageHPDrain] Tier {tier}, drain percentage: {drain}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", (val * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
    
    [System.Serializable]
    public class AltarMp_AttackIncrease : AltarMP
    {
        [SerializeField] private List<float> _percentage;

        public override void Apply()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            CLog.Log($"[AltarMp_AttackIncrease] Tier {tier}, attack added: {val}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", (val * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
    
    
    [System.Serializable]
    public class AltarMp_MightyBlock : AltarMP
    {
        [SerializeField] private List<int> _blocksCount;

        public override void Apply()
        {
            var tier = _tier >= _blocksCount.Count ? _blocksCount.Count - 1 : _tier;
            var val = _blocksCount[tier];
            CLog.Log($"[AltarMp_MightyBlock] Tier {tier}, blocks count: {val}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _blocksCount.Count ? _blocksCount.Count - 1 : _tier;
            var val = _blocksCount[tier];
            var d = _description.Replace("<val>", val.ToString());
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }

}