using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigJudgementOfLight : BaseSpellConfig
    {
        public List<CellsMask> cellsMasksByTear;
        public List<float> spellDamage;
        public List<float> physDamage;
    }
}