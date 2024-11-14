using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/bloodlust", fileName = "bloodlust", order = 206)]
    public class SpellProviderBloodLust : SpellProvider
    {
        [SerializeField] private SpellConfigBloodLust _config;

        public override void AddTo(GameObject target)
        {
            var components = target.GetComponent<HeroComponents>();
            if(components != null)
                AddToHero(components);
        }
        
        public override void AddToHero(HeroComponents components)
        {
            components.stats.FullManaListener = new SpellBloodLust(_config, components);
        }
        
        public override string GetDescription(GameObject target)
        {
            var str = base.GetDescription(target);
            str = str.Replace("<atks>", $"{_config.speedBonus*100}%");
            str = str.Replace("<time>", _config.duration.ToString());
            return str;
        }

        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;

    }
}