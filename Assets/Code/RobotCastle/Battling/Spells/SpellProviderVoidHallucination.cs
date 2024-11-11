using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Void Hallucination", fileName = "void_hallucination", order = 0)]
    public class SpellProviderVoidHallucination : SpellProvider
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
            components.stats.FullManaListener = new SpellVoidHallucination(_config, components);
        }

        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            str.Replace("<num>", _config.ghostsCount.ToString());
            str.Replace("<time>", _config.duration.ToString());

            var lvl = 0;
            if (target != null)
            {
                var stats = target.GetComponent<HeroStatsManager>();
                lvl = stats.MergeTier;
                str = str.Replace("<phys>", $"<color={HeroesConstants.ColorPhysDamage}>{_config.damageAdded[lvl]}</color>");
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{Mathf.RoundToInt(_config.damageSpMultiplier * stats.SpellPower.Get())}</color>");
            }
            else
            {
                str = str.Replace("<phys>", $"<color={HeroesConstants.ColorPhysDamage}>{_config.damageAdded[0]}</color>");
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{Mathf.RoundToInt(_config.damageSpMultiplier * 100)}%</color>");
            }
            return str;
        }
        
        [SerializeField] private SpellConfigVoidHallucination _config;

        
    }
}