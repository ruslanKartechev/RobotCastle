using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Soul Link", fileName = "soul_link", order = 0)]
    public class SpellProviderSoulLink : SpellProvider
    {
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellSoulLink(_config, components);
        }

        public override float manaMax => _config.manaMax;

        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigSoulLink _config;

    }
}