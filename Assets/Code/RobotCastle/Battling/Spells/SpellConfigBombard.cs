using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigBombard : BaseSpellConfig
    {
        public float loadDuration;
        public RInt spawnCount;
        public CoreItemData spawnUnit;
    }
}