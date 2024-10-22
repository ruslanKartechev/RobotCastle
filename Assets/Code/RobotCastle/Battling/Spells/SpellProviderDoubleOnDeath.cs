using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Double On Death", fileName = "double_on_death", order = 0)]
    public class SpellProviderDoubleOnDeath : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigCrescentSlash _config;
       

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroComponents view)
        {
            view.stats.FullManaListener = new SpellCrescentSlash(view, _config);
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