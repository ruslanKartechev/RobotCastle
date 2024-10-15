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
        
        public override void AddToHero(HeroComponents view)
        {
            view.stats.FullManaListener = new SpellCrescentSlash(view, _config);
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