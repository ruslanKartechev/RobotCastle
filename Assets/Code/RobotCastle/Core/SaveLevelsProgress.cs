namespace RobotCastle.Core
{
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
}