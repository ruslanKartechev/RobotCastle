using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Rain Of Bombs", fileName = "rain_of_bombs", order = 0)]
    public class SpellProviderRainOfBombs : SpellProvider
    {
        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellRainOfBombs(_config, components);
        }

        public override string GetDescription(GameObject target)
        {
            var msg = base.GetDescription(target);
            if (!target.TryGetComponent<HeroComponents>(out var components))
                return msg;
            var lvl = (int)HeroesManager.GetSpellTier(components.stats.MergeTier);
            var duration = _config.duration[lvl] + components.stats.SpellPower.Get() / 50f;;
            var atks = _config.attackSpeed[lvl];
            msg.Replace("<time>", $"{duration:N1}");
            msg.Replace("<atkSpeed>", Mathf.RoundToInt(atks * 100).ToString());
            return msg;
        }

        public override float manaMax => _config.manaMax;

        public override float manaStart => _config.manaStart;

        [SerializeField] private SpellConfigRainOfBombs _config;

    }
}