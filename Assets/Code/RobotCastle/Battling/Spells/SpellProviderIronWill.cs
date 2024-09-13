using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellProviderIronWill", fileName = "spell_iron_will", order = 0)]
    public class SpellProviderIronWill : SpellProvider
    {
        [SerializeField] private float _spellPower;

        public override void AddTo(GameObject target)
        { }
    }
}