using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigIronWill : BaseSpellConfig
    {
        public List<float> spellResist;
        public List<float> physResist;
    }
}