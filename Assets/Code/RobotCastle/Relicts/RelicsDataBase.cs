using System.Collections.Generic;

namespace RobotCastle.Relics
{
    [System.Serializable]
    public class RelicsDataBase
    {
        public List<int> playerLevelsToUnlockSlots;
        public Dictionary<string, RelicData> relicData;
    }
}