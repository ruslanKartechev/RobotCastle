using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigBlizzard : BaseSpellConfig
    {
        public List<CellsMask> cellsMasksByTear;
        public List<float> duration;
        public List<float> spellDamage;
        public List<float> physDamage;
    }
}