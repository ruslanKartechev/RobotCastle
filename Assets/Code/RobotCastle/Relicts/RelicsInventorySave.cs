using System.Collections.Generic;

namespace RobotCastle.Relics
{
    [System.Serializable]
    public class RelicsInventorySave
    {
        public int unlockedSlotsCount;
        public List<RelicSave> allRelics;

        public RelicsInventorySave(RelicsInventorySave other)
        {
            unlockedSlotsCount = other.unlockedSlotsCount;
            var count = other.allRelics.Count;
            allRelics = new List<RelicSave>(count);
            for (var i = 0; i < count; i++)
            {
                allRelics.Add(new RelicSave(other.allRelics[i]));
            }
        }
    }
}