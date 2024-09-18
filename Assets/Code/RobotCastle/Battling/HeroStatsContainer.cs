using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroStatsContainer : MonoBehaviour
    {
        private HeroStats _stats;
        private IAttackRange _range;
        public IAttackRange Range => _range;
        
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
        public float ProjectileSpeed { get; set; } = 10;
        
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

        public void Init(string id, int levelIndex, int mergeLevel)
        {
            HeroId = id;
            var db = ServiceLocator.Get<HeroesDatabase>();
            _stats = db.GetStatsForHero(id);
            SetLevel(levelIndex, mergeLevel);
            _range = AttackRangeFactory.GetAttackRangeById(id);
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
        
    }
}