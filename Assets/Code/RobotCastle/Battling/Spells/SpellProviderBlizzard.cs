using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProviderBlizzard", fileName = "spell_blizzard", order = 0)]
    public class SpellProviderBlizzard : SpellProvider
    {
        [SerializeField] private float _spellPower;

        public override void AddTo(GameObject target)
        { }
    }
}