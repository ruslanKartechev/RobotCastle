using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Summon Dead", fileName = "summon_dead", order = 0)]
    public class SpellProviderSummonDead : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
       

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellSummonDead(components, _config);
        }

        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            return str;
        }
        
        [SerializeField] private SpellConfigSummonDead _config;

    }
}