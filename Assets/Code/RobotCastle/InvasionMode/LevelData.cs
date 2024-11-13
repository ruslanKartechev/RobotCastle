using System.Collections.Generic;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class LevelData
    {
        public List<EliteItemsPreset> eliteItemsByLevel;
        public List<RoundData> levels;
        
        public LevelData(){}

        public LevelData(LevelData other)
        {
            var count = other.eliteItemsByLevel.Count;
            eliteItemsByLevel = new (count);
            for (var i = 0; i < count; i++)
                eliteItemsByLevel.Add(new EliteItemsPreset(other.eliteItemsByLevel[i]));
            count = other.levels.Count;
            levels = new List<RoundData>(count);
            foreach (var r in other.levels)
                levels.Add(new RoundData(r));
            
        }
    }
}