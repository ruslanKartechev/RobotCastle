using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProvider Crackle", fileName = "Crackle", order = 0)]
    public class SpellProviderCrackle : SpellProvider
    {
        [SerializeField] private SpellConfigCrackle _config;
        
        public override void AddTo(GameObject target) {        }

        public override void AddToHero(HeroComponents components) { }

        public override float manaMax => 100;
        public override float manaStart => 0;
    }
}