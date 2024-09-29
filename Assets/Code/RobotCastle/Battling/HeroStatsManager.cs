using System.Collections.Generic;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    // All stats have a list of "static modifiers"
    // I.e. modifiers that come from boosting items (swords, armor, etc.)
    // Health and mana reset is through interface. Default - health resets to full, mana resets to zero
    public class HeroStatsManager : MonoBehaviour
    {
        #region Static
        public static float GetStatByLevel(List<float> tableValues, int heroLevel, int mergeLevel)
            => tableValues[heroLevel] * HeroesConstants.TierStatMultipliers[mergeLevel];

        public static float GetStatByMergeLevel(List<float> tableValues, int mergeLevel) 
            => tableValues[mergeLevel];

        public static float GetMoveSpeed(List<float> tableValues) => tableValues[0] * HeroesConstants.SpeedStatFactor;
        #endregion

        public IAttackRange Range => _range;
        
        public IHealthReset HealthReset
        {
            get => _healthReset;
            set => _healthReset = value;
        }
        
        public IManaReset ManaResetAfterBattle
        {
            get => _manaResetAfterBattle;
            set => _manaResetAfterBattle = value;
        }

        public IManaReset ManaResetAfterFull { get; set; } = new ManaResetZero();
        
        public IFullManaListener FullManaListener
        {
            get => _fullManaListener;
            set => _fullManaListener = value;
        }

        public IManaAdder ManaAdder { get; set; }
        public IDamageCalculator DamageCalculator { get; set; }
        
        public float Shield = 0f;
        public Stat Attack {get; set;} = new Stat();
        public Stat AttackSpeed {get; set;} = new Stat();
        public Stat SpellPower {get; set;} = new Stat();
        public Stat MoveSpeed {get; set;} = new Stat();
        
        public Stat HealthMax {get; set;} = new Stat();
        public Stat HealthCurrent { get; set; } = new Stat();
        
        public Stat ManaMax {get; set;} = new Stat();
        public Stat ManaCurrent { get; set; } = new Stat();
        
        public Stat PhysicalResist { get; set; } = new Stat();
        public Stat MagicalResist { get; set; } = new Stat();
        
        public Stat PhysicalCritDamage { get; set; } = new Stat();
        public Stat MagicalCritDamage { get; set; } = new Stat();

        public Stat PhysicalCritChance { get; set; } = new Stat();
        public Stat MagicalCritChance { get; set; } = new Stat();

        
        public string HeroId { get; private set; }
        public int MergeTier { get; private set; }
        public int HeroLvl { get; private set; }
        public float ProjectileSpeed { get; set; } = 5;
        
        private HeroStats _stats;
        private IAttackRange _range;
        private IHealthReset _healthReset;
        private IManaReset _manaResetAfterBattle;
        private IFullManaListener _fullManaListener;
    

        public void LoadAndSetHeroStats(string id, int levelIndex, int mergeLevel)
        {
            HeroId = id;
            var db = ServiceLocator.Get<HeroesDatabase>();
            _stats = db.GetStatsForHero(id);
            _range = AttackRangeFactory.GetAttackRangeById(id);
            MergeTier = mergeLevel;
            HeroLvl = levelIndex;
            SetStatsToTable();
        }

        public void SetMergeLevel(int mergeLevel)
        {
            MergeTier = mergeLevel;
            SetStatsToTable();
        }

        public void SetStatsToTable()
        {
            HealthMax.SetBaseAndCurrent(GetStatByLevel(_stats.health, HeroLvl, MergeTier));
            HealthCurrent.SetBaseAndCurrent(HealthMax.Val);
            
            Attack.SetBaseAndCurrent(GetStatByLevel(_stats.attack, HeroLvl, MergeTier));
            Attack.SetBaseAndCurrent(GetStatByLevel(_stats.attack, HeroLvl, MergeTier));

            PhysicalResist.SetBaseAndCurrent(_stats.physicalResist[0]);
            MagicalResist.SetBaseAndCurrent(_stats.magicalResist[0]);

            PhysicalCritChance.SetBaseAndCurrent(_stats.physicalCritChance[0]);
            MagicalCritChance.SetBaseAndCurrent(_stats.magicalCritChance[0]);

            PhysicalCritDamage.SetBaseAndCurrent(_stats.physicalCritDamage[0]);
            MagicalCritDamage.SetBaseAndCurrent(_stats.magicalCritDamage[0]);
            
            AttackSpeed.SetBaseAndCurrent(GetStatByMergeLevel(_stats.attackSpeed, MergeTier));
            MoveSpeed.SetBaseAndCurrent(GetMoveSpeed(_stats.moveSpeed));
        }

        public void CheckManaFull()
        {
            if (ManaCurrent.Get() >= ManaMax.Get())
                _fullManaListener.OnFullMana(gameObject);
        }

        public void ClearDecorators()
        {
            // Log($"Clear All Decorators");
            SpellPower.ClearDecorators();
            HealthMax.ClearDecorators();
            HealthCurrent.ClearDecorators();
            MoveSpeed.ClearDecorators();
            Attack.ClearDecorators();
            AttackSpeed.ClearDecorators();
            ManaMax.ClearDecorators();
            ManaCurrent.ClearDecorators();
            PhysicalResist.ClearDecorators();
            MagicalResist.ClearDecorators();
        }

        [ContextMenu("LogAllDecorators")]
        public void LogAllDecorators()
        {
            var msg = "";
            Add("Attack", Attack.Decorators);
            Add("SpellPower", SpellPower.Decorators);
            Add("Attack Speed", AttackSpeed.Decorators);            
            Add("Move Speed", MoveSpeed.Decorators);            
            Add("Health", HealthCurrent.Decorators);            
            Add("Health_max", HealthMax.Decorators);
            Add("Phys DEF", PhysicalResist.Decorators);
            Add("Magic DEF", MagicalResist.Decorators);
            
            Log(msg);
            void Add(string statName, List<IStatDecorator> decorators)
            {
                msg += $"{statName} has {decorators.Count}: ";
                foreach (var dec in decorators)
                    msg += $"{dec.name} ";
                msg += "\n";
            }
        }
        

        private void Log(string msg) => CLog.LogWhite($"[Stats] [{gameObject.name}] {msg}");
        
    }
}