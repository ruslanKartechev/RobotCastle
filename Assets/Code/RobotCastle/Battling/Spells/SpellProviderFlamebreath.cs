using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider Flamebreath", fileName = "Flamebreath", order = 0)]
    public class SpellProviderFlamebreath : SpellProvider
    {
        [SerializeField] private SpellConfigFlamebreath _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}