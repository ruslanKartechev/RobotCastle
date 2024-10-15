using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider RainOfBombs", fileName = "RainOfBombs", order = 0)]
    public class SpellProviderRainOfBombs : SpellProvider
    {
        [SerializeField] private SpellConfigRainOfBombs _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}