using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Relics
{
    [CreateAssetMenu(menuName = "SO/Relics/Stat Modifier", fileName = "Relic Stat Modifier", order = 0)]
    public class RelicStatModifier : RelicModifier, IPlayerItemSpawnModifier
    {

        public float GetAtkMod() => _atkMod;

        public float GetAtkSpeedMod() => _atkSpeedMod;

        public float GetDEf() => _additionalDEF;

        public float GetHealth() => _maxHealthmod;

       
        public override string GetFullDescription() => "";

        public override void Apply()
        {
            CLog.Log($"Applying: {nameof(RelicStatModifier)}");   
            ServiceLocator.Get<IPlayerFactory>().AddModifier(this);
            if (_atkDecor == null)
            {
                _atkDecor = new StatDecorMult(_atkMod);
                _atkSpeedDecor = new StatDecorMult(_atkSpeedMod);
                _defDecor = new StatDecorAdd(_atkSpeedMod);
                _maxHealthDecor = new StatDecorMult(_maxHealthmod);
            }
        }

        public void OnNewItemSpawned(IItemView view)
        {
            if (view.itemData.core.type == ItemsIds.TypeHeroes)
            {
                var components = view.Transform.GetComponent<HeroComponents>();
                components.stats.Attack.AddPermanentDecorator(_atkDecor);
                components.stats.AttackSpeed.AddPermanentDecorator(_atkSpeedDecor);
                components.stats.HealthMax.AddPermanentDecorator(_maxHealthDecor);
                components.stats.PhysicalResist.AddPermanentDecorator(_defDecor);
                components.stats.MagicalResist.AddPermanentDecorator(_defDecor);
            }
        }
        
        [SerializeField] private float _atkMod;
        [SerializeField] private float _atkSpeedMod;
        [SerializeField] private float _additionalDEF;
        [SerializeField] private float _maxHealthmod;
        private StatDecorMult _atkDecor;
        private StatDecorMult _atkSpeedDecor;
        private StatDecorAdd _defDecor;
        private StatDecorMult _maxHealthDecor;
        

        private class StatDecorMult : IStatDecorator
        {
            public string name => "relic_mod_mult";

            public int order => 30;
            
            public float Decorate(float val) => val * multiplier;

            public StatDecorMult(float val) => multiplier = 1 + val;
            
            public float multiplier;
        }
        
        private class StatDecorAdd : IStatDecorator
        {
            public string name => "relic_mod_add";

            public int order => 30;
            
            public float Decorate(float val) => val + addedValue;

            public StatDecorAdd(float val) => addedValue = val;
            
            public float addedValue;
        }

        
    }
    
    
}