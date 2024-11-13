using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Backstab", fileName = "backstab", order = 14)]
    public class SpellProviderBackstab : SpellProvider
    {
        [SerializeField] private SpellConfigBackstab _config;

        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellBackstab(_config, components);
        }

        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        public override string GetDescription(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            var str = base.GetDescription(target);
            if (components == null)
            {
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>SP</color>");
                return str;
            }
            str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{components.stats.SpellPower.Get()}</color>");
            return str;
        }

    }
}