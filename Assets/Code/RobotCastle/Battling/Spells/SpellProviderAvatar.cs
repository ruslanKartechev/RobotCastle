using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider Avatar", fileName = "Avatar", order = 0)]
    public class SpellProviderAvatar : SpellProvider
    {
        [SerializeField] private SpellConfigAvatar _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroView view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}