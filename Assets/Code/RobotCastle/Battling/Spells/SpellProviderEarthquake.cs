using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/earthquake", fileName = "earthquake", order = 200)]
    public class SpellProviderEarthquake : SpellProvider
    {
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellEarthquake(_config, components);
        }

        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigEarthquake _config;

    }
}