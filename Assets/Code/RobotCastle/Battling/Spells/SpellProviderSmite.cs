using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/smite", fileName = "smite", order = 0)]
    public class SpellProviderSmite : SpellProvider
    {
        [SerializeField] private SpellConfigSmite _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}