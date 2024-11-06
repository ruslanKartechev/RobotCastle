using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider Crackle", fileName = "Crackle", order = 0)]
    public class SpellProviderCrackle : SpellProvider
    {
        
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

        public override void AddTo(GameObject target)
        {
            if (target.TryGetComponent<HeroComponents>(out var comp))
            {
                AddToHero(comp);
            }
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellCrackle(_config, components);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            var stats = target.GetComponent<HeroStatsManager>();
            var lvl = (int)HeroesManager.GetSpellTier(stats.MergeTier);
            str = str.Replace("<jumps>", _config.bounces[lvl].ToString());
            str = str.Replace("<dam>", _config.damageBase[lvl].ToString());
            str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{stats.SpellPower.Get()}</color>");
            return str;
        }

        [SerializeField] private SpellConfigCrackle _config;

    }
}