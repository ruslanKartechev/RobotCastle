using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigSummonDead : BaseSpellConfig
    {
        public List<CoreItemData> enemiesToSpawn;
    }
}