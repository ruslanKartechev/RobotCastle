using UnityEngine;

namespace RobotCastle.Battling
{
    [CreateAssetMenu(menuName = "SO/Spells/SpellHeadshot", fileName = "SpellHeadshot", order = 0)]
    public class SpellHeadshot : Spell
    {
        [SerializeField] private string _name;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private float _spellPower;
        
        public override Sprite GetIcon() => _sprite;
        
        public override string GetName() => _name;

        public override void AddTo(GameObject hero)
        {
        }

        public override string GetDescription(GameObject hero)
        {
            return $"Fires a powerful bullet at the target, dealing {_spellPower} damage.";
        }
    }
}