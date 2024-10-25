using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigPowerOfRock : BaseSpellConfig
    {
        public float hitDamage;
        public float addedDamageOnAttacks;
        public List<CellsMask> maskByTier;
        public float auraDuration = 3;
        public List<float> defByTier;
    }
}