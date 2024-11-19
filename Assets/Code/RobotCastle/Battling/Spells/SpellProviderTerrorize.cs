using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/terrorize", fileName = "terrorize", order = 210)]
    public class SpellProviderTerrorize : SpellProvider
    {
        [SerializeField] private SpellConfigTerrorize _config;
 
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellTerrorize(_config, components);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
             str = str.Replace("<time>", _config.fleeTime.ToString());
            return str;
        }

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

     
    }
}