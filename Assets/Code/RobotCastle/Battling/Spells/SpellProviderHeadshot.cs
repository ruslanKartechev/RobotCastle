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
        
        public override void AddToHero(HeroComponents view)
        {
            view.stats.FullManaListener = new SpellHeadshot(view, _config);
        }

        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            var lvl = 0;
            if (target != null)
            {
                var stats = target.GetComponent<HeroStatsManager>();
                lvl = stats.MergeTier;
                str = str.Replace("<phys>", $"<color={HeroesConstants.ColorPhysDamage}>{_config.physDamage[lvl]}</color>");
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{stats.SpellPower.Get()}</color>");
            }
            else
            {
                str = str.Replace("<phys>", $"<color={HeroesConstants.ColorPhysDamage}>{_config.physDamage[lvl]}</color>");
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{_config.spellDamage[lvl]}</color>");
            }
            return str;
        }

    }
}