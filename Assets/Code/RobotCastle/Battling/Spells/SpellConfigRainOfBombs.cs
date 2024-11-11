using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigRainOfBombs : BaseSpellConfig
    {
        public List<float> duration;
        public List<float> attackSpeed;
    }
}