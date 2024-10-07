using System.Collections.Generic;

namespace RobotCastle.Data
{
    [System.Serializable]
    public class XpDatabase
    {
        public List<int> castleXpLevels;
        public List<int> heroXpLevels; // 5, 8, 10, 20, 50, 100, 200, 200
        public List<int> heroesUpgradeCosts; // 40, 100, 200, 400, 1000, 2000, 4000    
        
        public Dictionary<string, float> xpGrantedByItem;
    }
}