using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider Flamebreath", fileName = "Flamebreath", order = 0)]
    public class SpellProviderFlamebreath : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

        public override void AddTo(GameObject target)
        {
            if (target.TryGetComponent<HeroComponents>(out var comp))
            {
                AddToHero(comp);
            }
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellFlamebreath(components, _config);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            var stats = target.GetComponent<HeroStatsManager>();
            str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{stats.SpellPower.Get()}</color>");
            return str;
        }
        
        [SerializeField] private SpellConfigFlamebreath _config;
    }
}