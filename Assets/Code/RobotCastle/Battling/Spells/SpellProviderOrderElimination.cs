using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/OrderElimination", fileName = "order_elimination", order = 40)]
    public class SpellProviderOrderElimination : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;


        public override void AddTo(GameObject target)
        {
            if(target.TryGetComponent<HeroComponents>(out var hero))
                AddToHero(hero);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellOrderElimination(_config, components);
        }

        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            // str.Replace("<num>", _config.ghostsCount.ToString());
            str.Replace("<maxNum>", _config.maxCount.ToString());
            str.Replace("<mag>", _config.normalDamage.ToString());
            return str;
        }
        
        [SerializeField] private SpellConfigOrderElimination _config;


    }
}