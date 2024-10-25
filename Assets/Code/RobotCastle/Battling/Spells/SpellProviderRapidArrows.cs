using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Rapid Arrows", fileName = "rapid_arrows", order = 10)]
    public class SpellProviderRapidArrows : SpellProvider
    {
        [SerializeField] private SpellConfigRapidArrows _config;

        public override void AddTo(GameObject target)
        {
            if (target.TryGetComponent<HeroComponents>(out var comp))
            {
                AddToHero(comp);
            }
        }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellRapidArrows(_config, components);
        }

        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
    }
}