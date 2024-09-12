using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellHeadshot", fileName = "SpellHeadshot", order = 0)]
    public class SpellHeadshot : ModifierProvider
    {
        [SerializeField] private float _spellPower;

        public override void AddTo(GameObject target)
        {
        }

        public override string GetDescription(GameObject target)
        {
            return $"Fires a powerful bullet at the target, dealing {_spellPower} damage.";
        }
    }
}