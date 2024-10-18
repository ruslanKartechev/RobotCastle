using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider RadianceOfLife", fileName = "radiance_of_life", order = 0)]
    public class SpellProviderRadianceOfLife : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigRadianceOfLife _config;

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroComponents view)
        {
            view.stats.FullManaListener= new SpellRadianceOfLife(view, _config);
        }
        
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            var lvl = 0;
            if (target != null)
            {
                var stats = target.GetComponent<HeroStatsManager>();
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{stats.SpellPower.Get()}</color>");
            }
            else
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{_config.healAmount[lvl]}</color>");
            return str;
        }
    }
}