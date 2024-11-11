using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigVoidHallucination : BaseSpellConfig
    {
        public int ghostsCount;
        public List<float> damageAdded;
        public float damageSpMultiplier;
        public float duration;

    }
}