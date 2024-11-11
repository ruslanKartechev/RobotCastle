using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/RecklessProtection", fileName = "reckless_protection", order = 0)]
    public class SpellProviderRecklessProtection : SpellProvider
    {
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellRecklessProtection(_config, components);
        }

        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;

        [SerializeField] private SpellConfigRecklessProtection _config;

    }
}