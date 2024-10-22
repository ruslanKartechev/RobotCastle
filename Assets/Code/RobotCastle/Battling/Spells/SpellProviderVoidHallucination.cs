using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/Void Hallucination", fileName = "void_hallucination", order = 0)]
    public class SpellProviderVoidHallucination : SpellProvider
    {
        [SerializeField] private SpellConfigVoidHallucination _config;

        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents view) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}