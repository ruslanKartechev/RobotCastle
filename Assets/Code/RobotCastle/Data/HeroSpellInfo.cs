namespace RobotCastle.Data
{
    [System.Serializable]
    public class HeroSpellInfo
    {
        public string mainSpellId;
        public string secondSpellId;
        public string thirdSpellId;
        
        public HeroSpellInfo(){}

        public HeroSpellInfo(HeroSpellInfo other)
        {
            mainSpellId = other.mainSpellId;
            secondSpellId = other.secondSpellId;
            thirdSpellId = other.thirdSpellId;
        }
    }
}