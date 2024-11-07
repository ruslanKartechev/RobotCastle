using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigFlamebreath : BaseSpellConfig
    {
        public float baseDamage;
        public int hitCount;
        public float hitDelay;
        public List<int> length;
    }
}