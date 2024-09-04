namespace RobotCastle.Core
{
    [System.Serializable]
    public class SavePlayerData
    {
        public int money;
        public int playerLevel;
        
        public SavePlayerData(){}

        public SavePlayerData(SavePlayerData other)
        {
            money = other.money;
            playerLevel = other.playerLevel;
        }
    }

    [System.Serializable]
    public class SaveLevelsProgress
    {
        public int lastLevelIndex;
        public int tower;
        public int levelsTotal;
        
        public SaveLevelsProgress(){}

        public SaveLevelsProgress(SaveLevelsProgress other)
        {
            lastLevelIndex = other.lastLevelIndex;
            tower = other.tower;
            levelsTotal = other.levelsTotal;
        }
    }

    [System.Serializable]
    public class SavePlayerHeroes
    {
	    
    }
}