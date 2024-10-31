using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigSoulLink : BaseSpellConfig
    {
        public List<int> maxDamage;
        public List<int> damageOnBroken;
        public float duration;

    }
}