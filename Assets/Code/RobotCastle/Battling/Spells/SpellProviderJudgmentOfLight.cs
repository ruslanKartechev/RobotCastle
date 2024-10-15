using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider JudgmentOfLight", fileName = "judgement_of_light", order = 0)]
    public class SpellProviderJudgmentOfLight : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigJudgementOfLight _config;

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroComponents view)
        {
            view.stats.FullManaListener = new SpellJudgementOfLight(view, _config);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            var stats = target.GetComponent<HeroStatsManager>();
            var lvl = stats.MergeTier;
            str = str.Replace("<phys>", $"<color={HeroesConstants.ColorPhysDamage}>{_config.physDamage[lvl]}</color>");
            str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{stats.SpellPower.Get()}</color>");
            return str;
        }
    }
}