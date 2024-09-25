using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigHeadshot : BaseSpellConfig
    {
        public List<float> spellDamage;
        public List<float> physDamage;
    }
}