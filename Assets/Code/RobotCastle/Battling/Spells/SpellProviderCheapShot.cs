using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/cheapshot", fileName = "cheapshot", order = 208)]
    public class SpellProviderCheapShot : SpellProvider
    {
        [SerializeField] private SpellConfigCheapShot _config;
 
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellCheapshot(_config, components);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            str = str.Replace("<time>", _config.stunTime.ToString());
            return str;
        }

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

    }
}