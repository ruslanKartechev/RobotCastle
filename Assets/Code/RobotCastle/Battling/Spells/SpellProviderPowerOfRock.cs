using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider PowerOfRock", fileName = "PowerOfRock", order = 0)]
    public class SpellProviderPowerOfRock : SpellProvider
    {
        [SerializeField] private SpellConfigPowerOfRock _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}