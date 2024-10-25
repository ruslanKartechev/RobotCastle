using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigAvatar : BaseSpellConfig
    {
        public float duration = 6;
        public float spellDamage;
        public float hpDrainPercent = 1;
        public List<float> defByTier;
        public List<CellsMask> cellsMasksByTear;

    }
}