﻿using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
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
        {
            var ind = heroLevel;
            if (ind >= tableValues.Count)
                ind = tableValues.Count - 1;
            return tableValues[ind] * HeroesConstants.TierStatMultipliers[mergeLevel];
        }

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

        public Stat MightyBlock { get; set; } = new Stat();
        public Stat Vampirism { get; set; } = new Stat();
        
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

        public float ManaGainPerAttack { get; set; } = 10;
        
        public string HeroId { get; private set; }
        public int MergeTier { get; private set; }
        public int HeroLvl { get; private set; }
        public float ProjectileSpeed { get; set; } = HeroesConstants.ProjectileSpeed;
        
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
            SpellPower.SetBaseAndCurrent(GetStatByLevel(_stats.spellPower, HeroLvl, MergeTier));

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
            Add("Attack", Attack);
            Add("SpellPower", SpellPower);
            Add("Attack Speed", AttackSpeed);            
            Add("Move Speed", MoveSpeed);            
            Add("Health", HealthCurrent);            
            Add("Health_max", HealthMax);
            Add("Phys DEF", PhysicalResist);
            Add("Magic DEF", MagicalResist);
            
            Log(msg);
            void Add(string statName, Stat stat)
            {
                msg += $"{statName} ";
                msg += stat.GetAllDecoratorsAsStr();
                msg += "\n";
            }
        }
        

        private void Log(string msg) => CLog.LogWhite($"[Stats] [{gameObject.name}] {msg}");
        
    }
}