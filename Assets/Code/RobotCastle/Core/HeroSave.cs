namespace RobotCastle.Core
{
    [System.Serializable]
    public class HeroSave
    {
        public string id;
        public int level;
        public int xp;
        public int xpForNext;
        public bool isUnlocked;
        
        public HeroSave(){}

        public HeroSave(HeroSave other)
        {
            id = other.id;
            level = other.level;
            xp = other.xp;
            xpForNext = other.xpForNext;
            isUnlocked = other.isUnlocked;
        }
    }
}