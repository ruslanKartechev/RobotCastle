using UnityEngine;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class SpellConfigBlizzard : BaseSpellConfig
    {
        
    }
    
    [CreateAssetMenu(menuName = "SO/Spells/SpellProviderBlizzard", fileName = "spell_blizzard", order = 0)]
    public class SpellProviderBlizzard : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

        [SerializeField] private SpellConfigBlizzard _config;
        
        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroView view)
        {
            view.Stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            view.Stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
        }
    }
}