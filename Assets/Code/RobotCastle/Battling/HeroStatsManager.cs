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
        public static float GetMaxHealth(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.health[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];

        public static float GetAttack(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.attack[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];

        public static float GetAttackSpeed(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.attackSpeed[mergeLevel];

        public static float GetPhysicalResist(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.physicalResist[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];

        public static float GetMagicalResist(HeroStats stats, int levelIndex, int mergeLevel) 
            => stats.magicalResist[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];
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
        
        public IManaReset ManaResetAfterFull { get; set; }
        
        public IFullManaListener FullManaListener
        {
            get => _fullManaListener;
            set => _fullManaListener = value;
        }

        public ISpellPowerGetter SpellPowerGetter { get; set; } = null;
        public List<IStatDecorator> SpellPowerDecorators = new (2);
        public IManaAdder ManaAdder { get; set; }
        
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
            HealthMax.SetBaseAndCurrent(GetMaxHealth(_stats, HeroLvl, MergeTier));
            HealthCurrent.SetBaseAndCurrent(HealthMax.Val);
            
            Attack.SetBaseAndCurrent(GetAttack(_stats, HeroLvl, MergeTier));
            AttackSpeed.SetBaseAndCurrent(GetAttackSpeed(_stats, HeroLvl, MergeTier));
            MoveSpeed.SetBaseAndCurrent(_stats.moveSpeed[0] * HeroesConfig.SpeedStatFactor);

            PhysicalResist.SetBaseAndCurrent(GetPhysicalResist(_stats, HeroLvl, MergeTier));
            MagicalResist.SetBaseAndCurrent(GetMagicalResist(_stats, HeroLvl, MergeTier));
        }

        public void CheckManaFull()
        {
            if (ManaCurrent.Val >= ManaMax.Val)
                _fullManaListener.OnFullMana(gameObject);
        }

        public void ClearDecorators()
        {
            Log($"Clear All Decorators");
            SpellPowerDecorators.Clear();
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
            Add("SpellPower", SpellPowerDecorators);
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