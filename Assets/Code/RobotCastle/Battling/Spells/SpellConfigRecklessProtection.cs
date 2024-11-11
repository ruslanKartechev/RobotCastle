using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigRecklessProtection : BaseSpellConfig
    {
        public float jumpTime = 1;
        public float jumpHeight = 3;
        public List<int> damage;
        public List<int> range;
        public int pushbackDistance = 2;
        public float pushbackTime = .5f;
        
    }
}