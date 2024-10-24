using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigSmite : BaseSpellConfig
    {
        public List<float> damagePhys;
        public List<float> damageMag;
        public int range;
        public float stunDuration;

    }
}