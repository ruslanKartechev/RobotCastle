using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class Chapter
    {
        public string id;
        public string viewName;
        public float moneyGoldReward;
        public int playerXpReward = 10;
        public int heroXpReward = 10;
        public string location;
        public List<DifficultyTier> tiers;
        public List<CoreItemData> chapterCompletedRewards;
        public LevelData levelData;
        public int prevTierRequired = 2;

    }
}