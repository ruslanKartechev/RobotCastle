namespace RobotCastle.Core
{
    [System.Serializable]
    public class TutorialSave
    {
        public bool enterPlay;
        public bool battle;
        public bool smelt;
        public bool win;
        public bool heroUpgrade;
        public bool heroSummon;
        public bool altars;
        
        public TutorialSave(){}

        public TutorialSave(TutorialSave other)
        {
            enterPlay = other.enterPlay;
            battle = other.battle;
            smelt = other.smelt;
            win = other.win;
            heroUpgrade = other.heroUpgrade;
            heroSummon = other.heroSummon;
            altars = other.altars;
        }
        
    }
}