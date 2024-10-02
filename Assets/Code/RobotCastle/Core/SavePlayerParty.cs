using System.Collections.Generic;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class SavePlayerParty
    {
        public SavePlayerParty(SavePlayerParty other)
        {
            maxCount = other.maxCount;
            var count = other.heroesIds.Count;
            heroesIds = new List<string>(count);
            for (var i = 0; i < count; i++)
            {
                heroesIds.Add(other.heroesIds[i]);
            }
        }
        
        public int maxCount = 6;
        public List<string> heroesIds;

        public bool CheckIfFull()
        {
            foreach (var id in heroesIds)
            {
                if (string.IsNullOrEmpty(id))
                    return false;
            }
            return true;
        }
    }
}