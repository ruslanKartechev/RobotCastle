using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/smite", fileName = "smite", order = 0)]
    public class SpellProviderSmite : SpellProvider
    {
        [SerializeField] private SpellConfigSmite _config;

        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellSmite(_config, components);
        }

        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
    }
}