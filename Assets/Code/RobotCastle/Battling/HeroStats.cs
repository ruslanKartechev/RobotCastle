using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class HeroStats
    {
        public List<float> health; // per level
        public List<float> spellPower; // per level
        public List<float> attack; // per level
        public List<float> mana; // per tier 
        public List<float> attackSpeed; // per tier 
        public List<float> moveSpeed; // const 
        public string rangeId;

        public HeroStats(){}
        
        public HeroStats(HeroStats other)
        {
            health = new List<float>(other.health);
            spellPower = new List<float>(other.spellPower);
            attack = new List<float>(other.attack);
            mana = new List<float>(other.mana);
            attackSpeed = new List<float>(other.attackSpeed);
            moveSpeed = new List<float>(other.moveSpeed);
            rangeId = other.rangeId;
        }
    }
}