using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider RadianceOfLight", fileName = "radiance_of_light", order = 0)]
    public class SpellProviderRadianceOfLight : SpellProvider
    {
        public override float manaMax => _config.manaMax;
        
        public override float manaStart => _config.manaStart;
        
        [SerializeField] private SpellConfigRadianceOfLife _config;

        public override void AddTo(GameObject target)
        { }
        
        public override void AddToHero(HeroView view)
        {
        }
    }
}