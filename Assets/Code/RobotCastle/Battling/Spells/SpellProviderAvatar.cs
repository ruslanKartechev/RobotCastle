using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Avatar", fileName = "avatar", order = 11)]
    public class SpellProviderAvatar : SpellProvider
    {
        [SerializeField] private SpellConfigAvatar _config;

        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellAvatar(_config, components);
        }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}