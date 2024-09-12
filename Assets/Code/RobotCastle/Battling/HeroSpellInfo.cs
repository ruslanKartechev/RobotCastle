namespace RobotCastle.Battling
{
    [System.Serializable]
    public class HeroSpellInfo
    {
        public string mainSpellId;
        
        public HeroSpellInfo(){}

        public HeroSpellInfo(HeroSpellInfo other)
        {
            mainSpellId = other.mainSpellId;
        }
    }
}