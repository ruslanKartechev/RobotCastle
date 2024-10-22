using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider RecklessProtection", fileName = "RecklessProtection", order = 0)]
    public class SpellProviderRecklessProtection : SpellProvider
    {
        [SerializeField] private SpellConfigRecklessProtection _config;
        public override void AddTo(GameObject target) {}

        public override void AddToHero(HeroComponents components) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}