using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigAnimateUndead : BaseSpellConfig
    {
        public float loadDuration;
        public RInt spawnCount;
        public CoreItemData spawnUnit;

    }
}