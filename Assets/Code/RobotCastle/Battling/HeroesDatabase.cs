using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class HeroesDatabase
    {
        public Dictionary<string, HeroInfo> info;

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


        public static Sprite GetHeroSprite(string spriteId)
        {
            return Resources.Load<Sprite>($"sprites/{spriteId}");
        }

    }
}