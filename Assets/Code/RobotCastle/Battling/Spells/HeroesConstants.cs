using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroesConstants
    {
        public const int PlayerHealthStart = 3;
        public const int PlayerTroopsStart = 3;
        public const float ManaGainDamageMultiplier = .2f;
        
        public const int DuelMaxDistance = 3;
        
        public const float SpeedStatFactor = 1f / 40;
        public const float SpeedStatToAnimationFactor = 1f / 150;
        public const float AddedMana = 18;

        public const float RotationSpeed = 900f;
        public const float ProjectileSpeed = 3.3f;
        
        public const string AnimId_Move = "Run";
        public const string AnimId_Idle = "Idle";
        public const string AnimId_Attack = "Attack";
        
        public static readonly int Anim_Attack = Animator.StringToHash(AnimId_Attack);
        public static readonly int Anim_Idle = Animator.StringToHash(AnimId_Idle);
        public static readonly int Anim_Move = Animator.StringToHash(AnimId_Move);

        public readonly static List<float> TierStatMultipliers = new() { 1f, 1.6f, 2.6f, 3.6f, 4.8f, 6.0f, 7.0f };
        // Tier 1 - (0, 1), Tier 2 - (2, 3), Tier 3 - (4-5), Tier 4 - (6)
        public static readonly List<int> SpellTiersByMergeLevel = new() {0, 2, 4, 6};

        public const string ColorAddedStats = "#FFFF11";
        public const string ColorPhysDamage = "#FFFF11";
        public const string ColorMagDamage = "#1111FF";

        public const string SpellFXPrefab_Blizzard = "prefabs/spells/blizzard";
        public const string SpellFXPrefab_JudgementOfLight = "prefabs/spells/judgement_of_light";
        public const string SpellFXPrefab_CrescentSlash = "prefabs/spells/crescent_slash";
        public const string SpellFXPrefab_RadianceOfLife = "prefabs/spells/radiance_of_life";
        public const string SpellFXPrefab_IronWill = "prefabs/spells/iron_will";

        

    }
}