using System.Collections.Generic;
using System.Globalization;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
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

        #if UNITY_EDITOR
        [ContextMenu("ConvertPercentage")]
        private void ConvertPercentage()
        {
            var perc = _mod1.Perc;
            for (var i = 0; i < perc.Count; i++)
            {
                var p = perc[i];
                if (p > 1)
                    p /= 100;
                perc[i] = p;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }


    [System.Serializable]
    public class AltarMp_DamageHPDrain : AltarMP, IPlayerItemSpawnModifier, IStatDecorator
    {
        [SerializeField] private List<float> _percentage;
        private float _val;

        public List<float> Perc => _percentage;
        
        public override void Apply()
        {
            if (_tier < 1) return;
            
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            _val = _percentage[tier];
            
            ServiceLocator.Get<IPlayerFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", (val * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == ItemsIds.TypeHeroes)
            {
                var components = view.Transform.GetComponent<HeroComponents>();
                components.stats.Vampirism.AddPermanentDecorator(this);
            }
        }

        public string name => "vampirism_from_altar";
        public int order => 1;
        public float Decorate(float val) => val + _val;

    }
    
    [System.Serializable]
    public class AltarMp_AttackIncrease : AltarMP, IPlayerItemSpawnModifier, IStatDecorator
    {
        [SerializeField] private List<float> _percentage;
        private float _val;

        public override void Apply()
        {
            if (_tier < 1) return;
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            _val = _percentage[tier];
            ServiceLocator.Get<IPlayerFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _percentage.Count ? _percentage.Count - 1 : _tier;
            var val = _percentage[tier];
            var d = _description.Replace("<val>", (val * 100).ToString(CultureInfo.InvariantCulture));
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == ItemsIds.TypeHeroes)
            {
                var components = view.Transform.GetComponent<HeroComponents>();
                components.stats.Attack.AddPermanentDecorator(this);
            }
        }

        public string name => "atk_from_altar";
        
        public int order => 100;
        
        public float Decorate(float val)
        {
            return val * (1 + _val);
        }
    }
    
    
    [System.Serializable]
    public class AltarMp_MightyBlock : AltarMP, IPlayerItemSpawnModifier
    {
        [SerializeField] private List<int> _blocksCount;

        public override void Apply()
        {
            if (_tier < 1) return;
            
            ServiceLocator.Get<IPlayerFactory>().AddModifier(this);
        }

        public override string GetShortDescription()
        {
            var tier = _tier >= _blocksCount.Count ? _blocksCount.Count - 1 : _tier;
            var val = _blocksCount[tier];
            var d = _description.Replace("<val>", val.ToString());
            return d;
        }

        public override string GetDetailedDescription() => _detailedDescription;
        
        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == ItemsIds.TypeHeroes)
            {
                var tier = _tier >= _blocksCount.Count ? _blocksCount.Count - 1 : _tier;
                var count = _blocksCount[tier];
                CLog.LogGreen($"[AltarMp_MightyBlock] Tier {tier}, blocks count: {count}");
                var components = view.Transform.GetComponent<HeroComponents>();
                components.preBattleRecurringMods.Add(new DamageTakeModMightyBlock(components, count));
            }
        }
    }

}