using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider smite", fileName = "Smite", order = 0)]
    public class SpellProviderSmite : SpellProvider
    {
        [SerializeField] private SpellConfigSmite _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroView view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}