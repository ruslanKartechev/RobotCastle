using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigCrescentSlash : BaseSpellConfig
    {
        public List<int> widthPerTier;
        public List<float> spellDamage;
    }
    
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
            view.Stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            view.Stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
        }
    }
}