using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroDamageSource : IDamageSource
    {
        public HeroDamageSource(HeroComponents components)
        {
            _components = components;
            _stats = components.stats;
        }
        
        public IBattleDamageStatsCollector StatsCollector { get; set; }
        
        public HeroDamageArgs CalculatePhysDamage()
        {
            var physDamage = _stats.Attack.Get();
            var chance = _stats.PhysicalCritChance.Get(); 
            if (chance > 0)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                if (rand <= chance)
                    physDamage *= 1 + _stats.PhysicalCritDamage.Get();
            }
            var args = new HeroDamageArgs(physDamage, EDamageType.Physical, _components);
            for (var i = 0; i < _calculationModifiers.Count; i++)
                args = _calculationModifiers[i].Apply(args);
            return args;
        }

        public HeroDamageArgs CalculateSpellDamage()
        {
            var magicDamage = _stats.SpellPower.Get();
            var chance = _stats.MagicalCritChance.Get();
            if (chance > 0)
            {
                var rand = UnityEngine.Random.Range(0f, 1f);
                if (rand <= chance)
                    magicDamage *= 1 + _stats.MagicalCritDamage.Get();
            }
            var args = new HeroDamageArgs(magicDamage, EDamageType.Magical, _components);
            for (var i = 0; i < _calculationModifiers.Count; i++)
                args = _calculationModifiers[i].Apply(args);
            return args;
        }
        

        public void DamagePhys(IDamageReceiver receiver)
        {
            var damage = CalculatePhysDamage();
            Damage(receiver, damage);
        }

        public void DamageSpellAndPhys(IDamageReceiver receiver)
        {
            var physDam = CalculatePhysDamage();
            var spellDam = CalculateSpellDamage();
            Damage(receiver, physDam);
            Damage(receiver, spellDam);
        }

        public void DamageSpell(IDamageReceiver receiver)
        {
            var spellDam = CalculateSpellDamage();
            Damage(receiver, spellDam);
        }

        public void DamageSpellAndPhys(float phys, float mag, IDamageReceiver receiver)
        {
            Damage(receiver, new HeroDamageArgs(phys, EDamageType.Physical, _components));
            Damage(receiver, new HeroDamageArgs(mag, EDamageType.Magical, _components));
        }


        public void AddModifier(IDamageCalculationModifier mod)
        {
            if(_calculationModifiers.Contains(mod) == false)
                _calculationModifiers.Add(mod);
        }

        public void RemoveModifier(IDamageCalculationModifier mod)
        {
            _calculationModifiers.Remove(mod);
        }

        public void AddModifier(IPostDamageModifier mod)
        {
            if(_postDamageModifiers.Contains(mod) == false)
                _postDamageModifiers.Add(mod);
        }

        public void RemoveModifier(IPostDamageModifier mod)
        {
            _postDamageModifiers.Remove(mod);
        }

        public void ClearDamageCalculationModifiers()
        {
            _calculationModifiers.Clear();
        }


        public void ClearPostDamageModifiers()
        {
            _postDamageModifiers.Clear();
        }
        
        private void Damage(IDamageReceiver receiver, HeroDamageArgs args)
        {
            var receivedArgs = receiver.TakeDamage(args);
            StatsCollector.AddDamageDealt(_components.GUID, args.type, (int)args.amount);
            var vamp = _components.stats.Vampirism.Get();
            if (vamp > 0)
            {
                var healthAdded = (int)(vamp * receivedArgs.amountReceived);
                CLog.Log($"Vampirism health: {healthAdded}");
                _components.stats.HealthCurrent.Val += healthAdded;
                ServiceLocator.Get<IDamageDisplay>().ShowVampirism(healthAdded, _components.pointVamp.position);
            }

            foreach (var mod in _postDamageModifiers)
                mod.Apply(receivedArgs);
        }

        
        private HeroComponents _components;
        private HeroStatsManager _stats;
        private List<IDamageCalculationModifier> _calculationModifiers = new (5);
        private List<IPostDamageModifier> _postDamageModifiers = new (5);

    }
}