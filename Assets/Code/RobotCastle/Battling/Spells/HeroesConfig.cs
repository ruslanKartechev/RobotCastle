using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroesConfig
    {
        public const float SpeedStatFactor = 1f / 40;
        public const float SpeedStatToAnimationFactor = 1f / 150;
        
        public const string AnimId_Move = "Run";
        public const string AnimId_Idle = "Idle";
        public const string AnimId_Attack = "Attack";
        
        public static readonly int Anim_Attack = Animator.StringToHash(AnimId_Attack);
        public static readonly int Anim_Idle = Animator.StringToHash(AnimId_Idle);

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