using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public class HeroesConfig
    {
        public readonly static List<float> TierStatMultipliers = new() { 1f, 1.6f, 2.6f, 3.6f, 4.8f, 6.0f, 7.0f };
        
        // Tier 1 - (0, 1), Tier 2 - (2, 3), Tier 3 - (4-5), Tier 4 - (6)
        public static readonly List<int> SpellTiersByMergeLevel = new() {0, 2, 4, 6};

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