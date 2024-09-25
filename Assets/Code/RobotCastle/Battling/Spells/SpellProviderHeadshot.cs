using UnityEngine;

namespace RobotCastle.Battling
{
    /// <summary>
    /// Headshot (MP 0/90)
    /// Fires a powerful bullet at the target, dealing 0/50/100/300 + (161/419/ /) SP damage.
    /// </summary>
    [CreateAssetMenu(menuName = "SO/Spells/SpellHeadshot", fileName = "spell_headshot", order = 0)]
    public class SpellProviderHeadshot : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigHeadshot _config;

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroView view)
        {
            view.Stats.ManaMax.SetBaseAndCurrent(_config.manaMax);
            view.Stats.ManaCurrent.SetBaseAndCurrent(_config.manaStart);
            view.Stats.ManaReset = new ManaResetZero();
            view.Stats.FullManaListener = new SpellHeadshot(view, _config);
        }

    }
}