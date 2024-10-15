using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Mage Altar", fileName = "Mage Altar", order = 4)]
    public class MageAltar : BasicAltar
    {
        [SerializeField] private AltarMp_InitialMp _mod1;
        [SerializeField] private AltarMp_FinalSp _mod2;
        [SerializeField] private AltarMp_MpUponKill _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){_mod1, _mod2, _mod3};
        }
    }


    [System.Serializable]
    public class AltarMp_InitialMp : AltarMP, IPlayerItemSpawnModifier, IStatDecorator
    {
        [SerializeField] private List<float> _percentage;
        private float _val;

        public override void Apply()
        {
            if (_tier < 1) return;

            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            _val = _percentage[tier];
            CLog.Log($"[AltarMp_InitialMp] Tier {tier}, initial mp percentage: {_val * 100}%");
            ServiceLocator.Get<IPlayerMergeItemsFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", $"{val * 100}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == MergeConstants.TypeHeroes)
            {
                var components = view.Transform.GetComponent<HeroComponents>();
                components.stats.ManaMax.AddPermanentDecorator(this);
                components.stats.ManaResetAfterBattle.Reset(components);
            }
        }

        public string name => "mp_from_altar";

        public int order => 100;
        
        public float Decorate(float val) => val * (1 + _val);
    }
    
    
    [System.Serializable]
    public class AltarMp_FinalSp : AltarMP, IPlayerItemSpawnModifier, IStatDecorator
    {
        [SerializeField] private List<float> _percentage;
        private float _val;


        public override void Apply()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            _val = _percentage[tier];
            CLog.Log($"[AltarMp_FinalSp] Tier {tier}, final spell power: {_val * 100}%");
            ServiceLocator.Get<IPlayerMergeItemsFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", $"{val*100}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == MergeConstants.TypeHeroes)
            {
                var components = view.Transform.GetComponent<HeroComponents>();
                components.stats.SpellPower.AddPermanentDecorator(this);
            }
        }

        public string name => "sp_from_altar";

        public int order => 100;
        
        public float Decorate(float val) => val * (1 + _val);
    }
    
    
       
    [System.Serializable]
    public class AltarMp_MpUponKill : AltarMP, IPlayerItemSpawnModifier
    {
        [SerializeField] private List<float> _percentage;
        private float _val;


        public override void Apply()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            _val = _percentage[tier];
            CLog.Log($"[AltarMp_MpUponKill] Tier {tier}, mp upon kill percentage: {_val * 100}%");
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", $"{val*100}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == MergeConstants.TypeHeroes)
            {
                var components = view.Transform.GetComponent<HeroComponents>();
                components.preBattleRecurringMods.Add(new RecurringPostDamageModAddMpPercentOnKill(components, _val));
            }
        }
    }
    
}