using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroSpellsContainer : MonoBehaviour, IModifiersContainer
    {
        [SerializeField] private List<ModifierProvider> _modifiers;
        private ModifierProvider _current;
        
        public List<ModifierProvider> modifiers => _modifiers;

        public ModifierProvider defaultSpell
        {
            get
            {
                if (_modifiers.Count > 0) return _modifiers[0];
                return null;
            }
        }
        public ModifierProvider currentSpell => _current;

        private void Awake()
        {
            if(_current == null)
                _current = defaultSpell;
        }
    }
}