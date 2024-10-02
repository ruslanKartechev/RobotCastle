using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider Backstab", fileName = "Backstab", order = 0)]
    public class SpellProviderBackstab : SpellProvider
    {
        [SerializeField] private SpellConfigBackstab _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroView view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}