using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/soul_burn", fileName = "soul_burn", order = 213)]
    public class SpellProviderSoulBurn : SpellProvider
    {
        
        public override void AddTo(GameObject target)
        {
            if (target.TryGetComponent<HeroComponents>(out var comp))
                AddToHero(comp);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellSoulBurn(_config, components);
        }

        [SerializeField] private SpellConfigSoulBurn _config;

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            str.Replace("<time>", _config.duration.ToString());
            return str;
        }
    }
}