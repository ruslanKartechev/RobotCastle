using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigRapidArrows : BaseSpellConfig
    {
        public float duration = 6f;
        public List<float> atkSpeedMultiplier;
    }
}