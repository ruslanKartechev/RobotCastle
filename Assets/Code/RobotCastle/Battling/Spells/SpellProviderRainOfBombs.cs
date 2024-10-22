using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Rain Of Bombs", fileName = "rain_of_bombs", order = 0)]
    public class SpellProviderRainOfBombs : SpellProvider
    {
        [SerializeField] private SpellConfigRainOfBombs _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents components) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}