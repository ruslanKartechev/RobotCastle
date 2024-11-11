using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling.Altars
{
    [CreateAssetMenu(menuName = "SO/Altars/Giant Altar", fileName = "Giant Altar", order = 3)]
    public class GiantAltar : BasicAltar
    {
        [SerializeField] private AltarMp_AddedDEF _mod1;
        [SerializeField] private AltarMp_HealthAdded _mod2;
        [SerializeField] private AltarMp_ReflectDamage _mod3;

        private void OnEnable()
        {
            _modifiers = new List<AltarMP>(){ _mod1, _mod2, _mod3 };
        }
    }


    [System.Serializable]
    public class AltarMp_AddedDEF : AltarMP, IPlayerItemSpawnModifier, IStatDecorator
    {
        [SerializeField] private List<float> _defence;
        private float _val;

        public override void Apply()
        {
            var tier = _tier >= _defence.Count ? _defence.Count - 1 : _tier;
            _val = _defence[tier];
            CLog.Log($"[AltarMp_AddedDEF] Tier {tier}, added DEF: {_val}");
            
            ServiceLocator.Get<IPlayerFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _defence.Count ? _defence.Count - 1 : _tier;
            var val = _defence[tier];
            var d = _description.Replace("<val>", $"{val}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == MergeConstants.TypeHeroes)
            {
                var components = view.Transform.GetComponent<HeroComponents>();
                components.stats.PhysicalResist.AddPermanentDecorator(this);
                components.stats.MagicalResist.AddPermanentDecorator(this);
            }
        }

        public string name => "DEF_from_altar";
        public int order => 1;
        public float Decorate(float val) => val + _val;
    }
    
    
    [System.Serializable]
    public class AltarMp_HealthAdded : AltarMP, IPlayerItemSpawnModifier, IStatDecorator
    {
        [SerializeField] private List<float> _healthPercent;
        private float _val;
        public override void Apply()
        {
            var tier = _tier >= _healthPercent.Count ? _healthPercent.Count - 1 : _tier;
            _val = _healthPercent[tier];
            CLog.Log($"[AltarMp_HealthAdded] Tier {tier}, added max health: {_val*100}%");
            ServiceLocator.Get<IPlayerFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _healthPercent.Count ? _healthPercent.Count - 1 : _tier;
            var val = _healthPercent[tier];
            var d = _description.Replace("<val>", $"{val*100}");
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
           if (view.itemData.core.type == MergeConstants.TypeHeroes)
           {
               var components = view.Transform.GetComponent<HeroComponents>();
               components.stats.HealthMax.AddPermanentDecorator(this);
               components.stats.HealthReset.Reset(components);
           }
        }

        public string name => "health_from_altar";
        public int order => 1;
        
        public float Decorate(float val) => val * (1 + _val);
    }
    
    
    
    [System.Serializable]
    public class AltarMp_ReflectDamage : AltarMP, IPlayerItemSpawnModifier
    {
        [SerializeField] private List<float> _percentage;
        private float _val;
        
        public override void Apply()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            _val = _percentage[tier];
            CLog.Log($"[AltarMp_ReflectDamage] Tier {tier}, reflected percentage: {_val}");
            ServiceLocator.Get<IPlayerFactory>().AddModifier(this);
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
                components.preBattleRecurringMods.Add(new DamageTakeModReflect(components, _val));
            }
        }
        
    }
}