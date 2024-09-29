using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public class DefaultDamageCalculator : IDamageCalculator
    {
        private HeroStatsManager _stats;
        private List<IDamageDecorator> _decorators;

        public DefaultDamageCalculator(HeroStatsManager stats)
        {
            _stats = stats;
        }
        
        public DamageArgs CalculateRegularAttackDamage()
        {
            var physDamage = _stats.Attack.Get();
            var chance = _stats.PhysicalCritChance.Get(); 
            if (chance > 0)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                if (rand <= chance)
                    physDamage *= 1 + _stats.PhysicalCritDamage.Get();
            }
            var args = new DamageArgs(physDamage, 0f);
            return args;
        }

        public DamageArgs CalculateSpellAttackDamage()
        {
            var physDamage = _stats.Attack.Get();
            var chance = _stats.PhysicalCritChance.Get(); 
            if (chance > 0)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                if (rand <= chance)
                    physDamage *= 1 + _stats.PhysicalCritDamage.Get();
            }
            var magicDamage = _stats.SpellPower.Get();
            chance = _stats.MagicalCritChance.Get();
            if (chance > 0)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                if (rand <= chance)
                    physDamage *= 1 + _stats.MagicalCritDamage.Get();
            }
            var args = new DamageArgs(physDamage, magicDamage);
            for (var ind = 0; ind < _decorators.Count; ind++)
            {
                var dec = _decorators[ind];
                args = dec.Apply(args);
            }

            return args;
        }
        
        public void AddDecorator(IDamageDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public void RemoveDecorator(IDamageDecorator decorator)
        {
            _decorators.Remove(decorator);
        }

        public void ClearAllDecorators()
        {
            _decorators.Clear();
        }

    }
}