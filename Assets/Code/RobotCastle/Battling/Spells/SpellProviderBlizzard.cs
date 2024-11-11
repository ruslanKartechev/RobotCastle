using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Blizzard", fileName = "spell_blizzard", order = 0)]
    public class SpellProviderBlizzard : SpellProvider
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
            components.stats.FullManaListener = new SpellBlizzard(components, _config);
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

        [SerializeField] private SpellConfigBlizzard _config;

    }
}