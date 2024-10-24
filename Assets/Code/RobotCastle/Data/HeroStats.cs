using System.Collections.Generic;

namespace RobotCastle.Data
{
    [System.Serializable]
    public class HeroStats
    {
        public List<float> health; // per level
        public List<float> attack; // per level
        public List<float> spellPower; // per tier 
        public List<float> attackSpeed; // per tier
        public List<float> physicalResist; // per tier
        public List<float> magicalResist; // per tier
        public List<float> physicalCritChance; // const
        public List<float> magicalCritChance; // const
        public List<float> physicalCritDamage; // const
        public List<float> magicalCritDamage; // const
        public List<float> moveSpeed; // const
        public List<float> physHpDrain; // const
        public List<float> magicHpDrain; // const
        public List<float> evasion; // const
        
        public string rangeId;

        public HeroStats()
        {
            
        }
        
        public HeroStats(HeroStats other)
        {
            health = new List<float>(other.health);
            attack = new List<float>(other.attack);
            spellPower = new List<float>(other.spellPower);
            attackSpeed = new List<float>(other.attackSpeed);
            moveSpeed = new List<float>(other.moveSpeed);
            physicalResist = new List<float>(other.physicalResist);
            magicalResist = new List<float>(other.magicalResist);
            physicalCritChance = new List<float>(other.physicalCritChance);
            magicalCritChance = new List<float>(other.magicalCritChance);
            physicalCritDamage = new List<float>(other.physicalCritDamage);
            magicalCritDamage = new List<float>(other.magicalCritDamage);
            physHpDrain = new List<float>(other.physHpDrain);
            magicHpDrain = new List<float>(other.magicHpDrain);
            evasion = new List<float>(other.evasion);
            rangeId = other.rangeId;
        }
    }
}