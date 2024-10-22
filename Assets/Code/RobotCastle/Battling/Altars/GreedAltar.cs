using System.Collections.Generic;
using RobotCastle.Battling.MerchantOffer;
using RobotCastle.Core;
using RobotCastle.InvasionMode;
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
            if (_tier < 1) return;
            var tier = _tier >= _money.Count ? _money.Count - 1 : _tier;
            var val = (int)_money[tier];
            CLog.Log($"[AltarMp_InitialMoney] Tier {tier}, added money: {val}");
            ServiceLocator.Get<IBattleStartData>().AddMoney(val);
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
    public class AltarMp_EliteRoundSilver : AltarMP, IRoundModifier
    {
        [SerializeField] private List<float> _money;

        public override void Apply()
        {
            if (_tier < 1) return;
            ServiceLocator.Get<BattleManager>().AddRoundModifier(this);
            CLog.Log("[AltarMp_EliteRoundSilver] Applied");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _money.Count ? _money.Count - 1 : _tier;
            var val = _money[tier];
            var d = _description.Replace("<val>", $"{val}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;

        public void OnRoundSet(BattleManager battleManager)
        {
            if (battleManager.currentRound.roundType == RoundType.EliteEnemy)
            {
                var tier = _tier >= _money.Count ? _money.Count - 1 : _tier;
                var val = (int)_money[tier];
                CLog.LogGreen($"[AltarMp_EliteRoundSilver] Tier {tier}, elite level money: {val}");
                ServiceLocator.Get<GameMoney>().AddMoney(val);
            }
        }

        public void OnRoundCompleted(BattleManager battleManager)
        { }
    }
    
    
    
    [System.Serializable]
    public class AltarMp_MerchantOfferSale : AltarMP
    {
        [SerializeField] private List<float> _sale;

        public override void Apply()
        {
            if (_tier < 1) return;

            var tier = _tier >= _sale.Count ? _sale.Count - 1 : _tier;
            var sale = _sale[tier];
            CLog.Log($"[AltarMp_MerchantOfferSale] Tier {tier}, sale: {sale * 100}%");
            ServiceLocator.Get<MerchantOfferManager>().Sale += sale;
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