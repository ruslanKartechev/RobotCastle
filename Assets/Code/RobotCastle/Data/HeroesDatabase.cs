using System.Collections.Generic;

namespace RobotCastle.Data
{
    [System.Serializable]
    public class HeroesDatabase
    {
        public Dictionary<string, HeroInfo> info;
        public List<int> xpForLevels = new(20);

        public HeroesDatabase()
        {
            info = new Dictionary<string, HeroInfo>(50);
        }

        public HeroStats GetStatsForHero(string id)
        {
            return info[id].stats;
        }
        
        public HeroViewInfo GetHeroViewInfo(string id)
        {
            return info[id].viewInfo;
        }
        
        public HeroSpellInfo GetHeroSpellInfo(string id)
        {
            return info[id].spellInfo;
        }

        public HeroInfo GetHeroInfo(string id)
        {
            return info[id];
        }

    }
}