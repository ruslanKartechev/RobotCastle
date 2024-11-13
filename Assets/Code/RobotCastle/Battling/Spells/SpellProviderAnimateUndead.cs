using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/animate_undead", fileName = "animate_undead", order = 203)]
    public class SpellProviderAnimateUndead : SpellProvider
    {
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }

        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellRaiseTheDead(_config, components);
        }

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigRaiseTheDead _config;
    }
}