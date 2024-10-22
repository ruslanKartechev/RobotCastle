using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Rapid Arrows", fileName = "rapid_arrows", order = 0)]
    public class SpellProviderRapidArrows : SpellProvider
    {
        [SerializeField] private SpellConfigRapidArrows _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents components) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}