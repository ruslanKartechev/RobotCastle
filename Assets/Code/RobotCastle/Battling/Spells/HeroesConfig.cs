using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public class HeroesConfig
    {
        public static readonly List<int> SpellTiersByMergeLevel = new() { 1, 3, 5, 6 };

        public static int GetSpellTier(int mergeLevel)
        {
            for (var i = SpellTiersByMergeLevel.Count - 1; i >= 0; i--)
            {
                if (mergeLevel >= SpellTiersByMergeLevel[i])
                    return i;
            }
            return 0;
        }
    }
}