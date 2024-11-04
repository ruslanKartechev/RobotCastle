using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigCrescentSlash : BaseSpellConfig
    {
        public float speed;
        public List<CellsMask> cellsMasksByTear;
        public List<float> spellDamage;
        public List<float> physDamage;
        public float animaDelaySec = .33f;
    }
}