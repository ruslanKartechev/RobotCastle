using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider RapidArrows", fileName = "RapidArrows", order = 0)]
    public class SpellProviderRapidArrows : SpellProvider
    {
        [SerializeField] private SpellConfigRapidArrows _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroView view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}