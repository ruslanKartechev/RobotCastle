using TMPro;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider IronWill", fileName = "spell_iron_will", order = 0)]
    public class SpellProviderIronWill : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigIronWill _config;

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroView view)
        {
            view.stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            view.stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
        }
    }
}