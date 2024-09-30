using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigRadianceOfLife : BaseSpellConfig
    {
        public List<CellsMask> cellsMasksByTear;
        public List<float> healAmount;
    }
}