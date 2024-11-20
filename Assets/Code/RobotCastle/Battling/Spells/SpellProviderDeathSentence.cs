using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/death_sentence", fileName = "death_sentence", order = 215)]
    public class SpellProviderDeathSentence : SpellProvider
    {
        public override void AddTo(GameObject target)
        {
            if (target.TryGetComponent<HeroComponents>(out var comp))
                AddToHero(comp);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellDeathSentence(_config, components);
        }

        [SerializeField] private SpellConfigDeathSentence _config;

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            str = str.Replace("<phys>", $"<color={HeroesConstants.ColorPhysDamage}>{(int)_config.physDamage}</color>");
            if (target.TryGetComponent<HeroStatsManager>(out var stats))
            {
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{(int)stats.SpellPower.Get()}</color>");
            }
            else
            {
                str = str.Replace("<mag>", $"<color={HeroesConstants.ColorMagDamage}>{700}</color>");
            }
            return str;
        }
    }
}