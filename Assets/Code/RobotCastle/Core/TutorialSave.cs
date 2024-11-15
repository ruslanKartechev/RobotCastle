namespace RobotCastle.Core
{
    [System.Serializable]
    public class TutorialSave
    {
        public bool enterPlay;
        public bool battle;
        public bool merge;
        
        public bool win;
        public bool heroUpgrade;
        public bool heroSummon;
        public bool difficultyPick;
        
        public TutorialSave(){}

        public TutorialSave(TutorialSave other)
        {
            enterPlay = other.enterPlay;
            battle = other.battle;
            merge = other.merge;
            win = other.win;
            heroUpgrade = other.heroUpgrade;
            heroSummon = other.heroSummon;
            difficultyPick = other.difficultyPick;
        }
        
    }
}