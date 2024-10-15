using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroDamageSource : IDamageSource
    {
        private HeroView _view;
        private HeroStatsManager _stats;

        private List<IDamageDecorator> _decorators;

        public HeroDamageSource(HeroView view)
        {
            _view = view;
            _stats = view.stats;
            _decorators = new(10);
        }
        
        public DamageArgs CalculatePhysDamage()
        {
            var physDamage = _stats.Attack.Get();
            var chance = _stats.PhysicalCritChance.Get(); 
            if (chance > 0)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                if (rand <= chance)
                    physDamage *= 1 + _stats.PhysicalCritDamage.Get();
            }
            var args = new DamageArgs(physDamage, 0f, this);
            for (var ind = 0; ind < _decorators.Count; ind++)
            {
                var dec = _decorators[ind];
                args = dec.Apply(args);
            }
            return args;
        }

        public DamageArgs CalculateSpellAndPhysDamage()
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
            var args = new DamageArgs(physDamage, magicDamage, this);
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

        public void DamagePhys(IDamageReceiver receiver)
        {
            var damage = CalculatePhysDamage();
            Damage(receiver, damage);
        }

        public void DamageSpellAndPhys(IDamageReceiver receiver)
        {
            var damage = CalculateSpellAndPhysDamage();
            Damage(receiver, damage);
        }

        private void Damage(IDamageReceiver receiver, DamageArgs args)
        {
            var received = receiver.TakeDamage(args);
            CLog.LogGreen($"Actually received: {received.physDamage}, {received.magicDamage}");
            
        }

        public void ReflectDamageBack()
        {
            CLog.Log($"Damage reflected back !!!");
            
        }
    }
}