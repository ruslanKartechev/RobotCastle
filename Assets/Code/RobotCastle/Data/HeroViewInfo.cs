namespace RobotCastle.Data
{
    [System.Serializable]
    public class HeroViewInfo
    {
        public string name;
        public string iconId;

        public HeroViewInfo(){}
        
        public HeroViewInfo(HeroViewInfo other)
        {
            name = other.name;
            iconId = other.iconId;
        }
    }
}