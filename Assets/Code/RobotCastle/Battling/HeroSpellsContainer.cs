using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroSpellsContainer : MonoBehaviour, IModifiersContainer
    {
        [SerializeField] private List<ModifierProvider> _modifiers;
        private ModifierProvider _current;
        
        public List<ModifierProvider> modifiers => _modifiers;
        public ModifierProvider defaultSpell => _modifiers[0];
        public ModifierProvider currentSpell => _current;

        private void Awake()
        {
            _current = defaultSpell;
        }
    }
}