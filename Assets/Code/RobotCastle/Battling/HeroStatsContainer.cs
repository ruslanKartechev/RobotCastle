using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IHealthReset
    {
        void Reset(HeroView heroView);
    }

    public interface IManaReset
    {
        void Reset(HeroView heroView);
    }

    public class HealthResetFull : IHealthReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.Stats.HealthCurrent.SetBaseAndCurrent(heroView.Stats.HealthMax.Val);
        }
    }

    public class ManaResetFull : IManaReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.Stats.ManaCurrent.SetBaseAndCurrent(heroView.Stats.ManaMax.Val);
        }
    }
    
    public class ManaResetZero : IManaReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.Stats.ManaCurrent.SetBaseAndCurrent(0);
        }
    }

    public class DefaultFullManaAction : IFullManaListener
    {
        public void OnFullMana(GameObject heroGo)
        {
            var stats = heroGo.GetComponent<HeroStatsContainer>();
            stats.ManaCurrent.SetBaseAndCurrent(0);
        }
    }

    public interface IFullManaListener
    {
        void OnFullMana(GameObject heroGo);
    }
    
    public class TestSpellActivator : IFullManaListener
    {
        public void OnFullMana(GameObject heroGo)
        {
            var hero = heroGo.GetComponent<IHeroController>();
            
        }
    }
    
    // All stats have a list of "static modifiers"
    // I.e. modifiers that come from boosting items (swords, armor, etc.)
    // Health and mana reset is through interface. Default - health resets to full, mana resets to zero
    public class HeroStatsContainer : MonoBehaviour
    {
        
        public static float GetMaxHealth(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.health[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];

        public static float GetMana(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.mana[mergeLevel];

        public static float GetAttack(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.attack[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];

        public static float GetSpellPower(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.spellPower[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];

        public static float GetAttackSpeed(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.attackSpeed[mergeLevel];

        public static float GetPhysicalResist(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.physicalResist[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];

        public static float GetMagicalResist(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.magicalResist[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];
        
        
 
        public IAttackRange Range => _range;
        
        public IHealthReset HealthReset
        {
            get => _healthReset;
            set => _healthReset = value;
        }
        
        public IManaReset ManaReset
        {
            get => _manaReset;
            set => _manaReset = value;
        }

        public IFullManaListener FullManaListener
        {
            get => _fullManaListener;
            set => _fullManaListener = value;
        }
        
        
        public Stat SpellPower {get; set;} = new Stat();
        public Stat Attack {get; set;} = new Stat();
        public Stat AttackSpeed {get; set;} = new Stat();
        public Stat MoveSpeed {get; set;} = new Stat();
        
        public Stat HealthMax {get; set;} = new Stat();
        public Stat HealthCurrent { get; set; } = new Stat();
        
        public Stat ManaMax {get; set;} = new Stat();
        public Stat ManaCurrent { get; set; } = new Stat();
        
        public Stat PhysicalResist { get; set; } = new Stat();
        public Stat MagicalResist { get; set; } = new Stat();
        
        public string HeroId { get; private set; }
        public int MergeTier { get; private set; }
        public int HeroLvl { get; private set; }
        public float ProjectileSpeed { get; set; } = 5;
        
        private HeroStats _stats;
        private IAttackRange _range;
        private IHealthReset _healthReset;
        private IManaReset _manaReset;
        private IFullManaListener _fullManaListener;
        

        public void Init(string id, int levelIndex, int mergeLevel)
        {
            HeroId = id;
            var db = ServiceLocator.Get<HeroesDatabase>();
            _stats = db.GetStatsForHero(id);
            SetLevel(levelIndex, mergeLevel);
            _range = AttackRangeFactory.GetAttackRangeById(id);
        }

        public void ResetHealth()
        {
            HealthMax.Val = HealthMax.BaseVal;
            HealthCurrent.Val = HealthMax.Val;
        }

        public void ResetMana()
        {
            ManaMax.Val = ManaMax.BaseVal;
            ManaCurrent.Val = ManaMax.Val;
        }
        
        public void SetLevel(int levelIndex, int mergeLevel)
        {
            MergeTier = mergeLevel;
            HeroLvl = levelIndex;
            UpdateStats();
        }
        
        public void SetMergeLevel(int mergeLevel)
        {
            MergeTier = mergeLevel;
            UpdateStats();
        }

        public void UpdateStats()
        {
            HealthMax.SetBaseAndCurrent(GetMaxHealth(_stats, HeroLvl, MergeTier));
            HealthCurrent.SetBaseAndCurrent(HealthMax.Val);
            
            ManaMax.SetBaseAndCurrent(GetMana(_stats, HeroLvl, MergeTier));
            ManaCurrent.SetBaseAndCurrent(ManaMax.Val);
            
            SpellPower.SetBaseAndCurrent(GetSpellPower(_stats, HeroLvl, MergeTier));
            Attack.SetBaseAndCurrent(GetAttack(_stats, HeroLvl, MergeTier));
            AttackSpeed.SetBaseAndCurrent(GetAttackSpeed(_stats, HeroLvl, MergeTier));
            MoveSpeed.SetBaseAndCurrent(_stats.moveSpeed[0] * HeroesConfig.SpeedStatFactor);

            PhysicalResist.SetBaseAndCurrent(GetPhysicalResist(_stats, HeroLvl, MergeTier));
            MagicalResist.SetBaseAndCurrent(GetMagicalResist(_stats, HeroLvl, MergeTier));
        }

        public void AddMana(float added)
        {
            ManaCurrent.Val += added;
            if (ManaCurrent.Val >= ManaMax.Val)
            {
                _fullManaListener.OnFullMana(gameObject);
            }
        }
    }
}