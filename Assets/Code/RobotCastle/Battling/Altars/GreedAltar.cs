using System.Collections.Generic;
using SleepDev;
using UnityEngine;
namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Greed Altar", fileName = "Greed Altar", order = 5)]
    public class GreedAltar : BasicAltar
    {
        [SerializeField] private AltarMp_InitialMoney _mod1;
        [SerializeField] private AltarMp_EliteRoundSilver _mod2;
        [SerializeField] private AltarMp_MerchantOfferSale _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }
    
    
    [System.Serializable]
    public class AltarMp_InitialMoney : AltarMP
    {
        [SerializeField] private List<float> _money;


        public override void Apply()
        {
            var tier = _tier >= _money.Count ? _money.Count - 1 : _tier;
            var val = _money[tier];
            CLog.Log($"[AltarMp_DamageHPDrain] Tier {tier}, initial money: {val}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _money.Count ? _money.Count - 1 : _tier;
            var val = _money[tier];
            var d = _description.Replace("<val>", $"{val}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }

    
    [System.Serializable]
    public class AltarMp_EliteRoundSilver : AltarMP
    {
        [SerializeField] private List<float> _money;


        public override void Apply()
        {
            var tier = _tier >= _money.Count ? _money.Count - 1 : _tier;
            var val = _money[tier];
            CLog.Log($"[AltarMp_DamageHPDrain] Tier {tier}, elite level money: {val}");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _money.Count ? _money.Count - 1 : _tier;
            var val = _money[tier];
            var d = _description.Replace("<val>", $"{val}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
    
    
    
    [System.Serializable]
    public class AltarMp_MerchantOfferSale : AltarMP
    {
        [SerializeField] private List<float> _sale;

        public override void Apply()
        {
            var tier = _tier >= _sale.Count ? _sale.Count - 1 : _tier;
            var sale = _sale[tier];
            CLog.Log($"[AltarMp_DamageHPDrain] Tier {tier}, sale: {sale*100}%");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _sale.Count ? _sale.Count - 1 : _tier;
            var sale = _sale[tier];
            var d = _description.Replace("<val>", $"{sale*100}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
    }
}