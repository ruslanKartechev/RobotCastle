using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider CrescentSlash", fileName = "crescent_slash", order = 0)]
    public class SpellProviderCrescentSlash : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigCrescentSlash _config;
       

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroView view)
        {
            view.stats.FullManaListener = new SpellCrescentSlash(view, _config);
        }

      
    }
}