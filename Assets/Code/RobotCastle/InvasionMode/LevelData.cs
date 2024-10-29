using System.Collections.Generic;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class LevelData
    {
        public List<EliteItemsPreset> eliteItemsByLevel;
        public List<RoundData> levels;
    }
}