using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroStatsContainer : MonoBehaviour
    {
        public Stat SpellPower {get; set;} = new Stat();
        public Stat Attack {get; set;} = new Stat();
        public Stat AttackSpeed {get; set;} = new Stat();
        public Stat MoveSpeed {get; set;} = new Stat();
        
        public Stat HealthMax {get; set;} = new Stat();
        public Stat HealthCurrent { get; set; } = new Stat();
        
        public Stat ManaMax {get; set;} = new Stat();
        public Stat ManaCurrent { get; set; } = new Stat();
        
        public string HeroId { get; private set; }
        public int MergeTier { get; private set; }
        public int HeroLvl { get; private set; }


        public static float GetMaxHealth(HeroStats stats, int levelIndex, int mergeLevel)
        {
            return stats.health[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];
        }
        
        public static float GetMana(HeroStats stats, int levelIndex, int mergeLevel)
        {
            return stats.mana[mergeLevel];
        }
        
        public static float GetAttack(HeroStats stats, int levelIndex, int mergeLevel)
        {
            return stats.attack[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];
        }
        
        public static float GetSpellPower(HeroStats stats, int levelIndex, int mergeLevel)
        {
            return stats.spellPower[levelIndex] * HeroesConfig.TierStatMultipliers[mergeLevel];
        }
        
        public static float GetAttackSpeed(HeroStats stats, int levelIndex, int mergeLevel)
        {
            return stats.attackSpeed[mergeLevel];
        }
        
        private HeroStats _stats;
        private IAttackRange _range;

        public void Init(string id, int levelIndex, int mergeLevel)
        {
            HeroId = id;
            var db = ServiceLocator.Get<HeroesDatabase>();
            _stats = db.GetStatsForHero(id);
            SetLevel(levelIndex, mergeLevel);
            _range = AttackRangeFactory.GetAttackRangeById(_stats.rangeId);
        }
        
        public void SetLevel(int levelIndex, int mergeLevel)
        {
            HealthMax.SetBaseAndCurrent(GetMaxHealth(_stats, levelIndex, mergeLevel));
            HealthCurrent.SetBaseAndCurrent(HealthMax.Val);
            
            ManaMax.SetBaseAndCurrent(GetMana(_stats, levelIndex, mergeLevel));
            ManaCurrent.SetBaseAndCurrent(ManaMax.Val);
            
            SpellPower.SetBaseAndCurrent(GetSpellPower(_stats, levelIndex, mergeLevel));
            Attack.SetBaseAndCurrent(GetAttack(_stats, levelIndex, mergeLevel));
            AttackSpeed.SetBaseAndCurrent(GetAttackSpeed(_stats, levelIndex, mergeLevel));
            MoveSpeed.SetBaseAndCurrent(_stats.moveSpeed[0]);
            
            MergeTier = mergeLevel;
            HeroLvl = levelIndex;
        }
        
        public void SetMergeLevel(int mergeLevel)
        {
            HealthMax.SetBaseAndCurrent(GetMaxHealth(_stats, HeroLvl, mergeLevel));
            HealthCurrent.SetBaseAndCurrent(HealthMax.Val);
            
            ManaMax.SetBaseAndCurrent(GetMana(_stats, HeroLvl, mergeLevel));
            ManaCurrent.SetBaseAndCurrent(ManaMax.Val);
            
            SpellPower.SetBaseAndCurrent(GetSpellPower(_stats, HeroLvl, mergeLevel));
            Attack.SetBaseAndCurrent(GetAttack(_stats, HeroLvl, mergeLevel));
            AttackSpeed.SetBaseAndCurrent(GetAttackSpeed(_stats, HeroLvl, mergeLevel));
            MoveSpeed.SetBaseAndCurrent(_stats.moveSpeed[0]);
            MergeTier = mergeLevel;
        }
        
    }
}