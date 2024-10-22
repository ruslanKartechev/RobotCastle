using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Power Of Rock", fileName = "power_of_rock", order = 0)]
    public class SpellProviderPowerOfRock : SpellProvider
    {
        [SerializeField] private SpellConfigPowerOfRock _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents components) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}