using System.Collections.Generic;

namespace RobotCastle.Relicts
{
    [System.Serializable]
    public class RelicsDataBase
    {
        public List<int> playerLevelsToUnlockSlots;
        public Dictionary<string, RelicData> relicData;
    }
}