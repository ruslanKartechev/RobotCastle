namespace RobotCastle.Battling
{
    [System.Serializable]
    public class HeroInfo
    {
        public HeroStats stats;
        public HeroSpellInfo spellInfo;
        public HeroViewInfo viewInfo;
        
        public HeroInfo(){}

        public HeroInfo(HeroInfo other)
        {
            stats = new HeroStats(other.stats);
            spellInfo = new HeroSpellInfo(other.spellInfo);
            viewInfo = new HeroViewInfo(other.viewInfo);
        }
    }
}