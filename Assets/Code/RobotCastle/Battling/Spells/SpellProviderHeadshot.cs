using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellHeadshot", fileName = "spell_headshot", order = 0)]
    public class SpellProviderHeadshot : SpellProvider
    {
        [SerializeField] private float _spellPower;

        public override void AddTo(GameObject target)
        {
        }
    }
}