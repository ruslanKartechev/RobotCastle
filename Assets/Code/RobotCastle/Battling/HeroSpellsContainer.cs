using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Battling
{
    public class HeroSpellsContainer : IModifiersContainer
    {
        private List<ModifierProvider> _modifiers = new (5);
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

        public HeroSpellsContainer()
        { }

        public ModifierProvider GetCurrentSpell()
        {
            if (_modifiers.Count == 0) return null;
            return _modifiers[0];
        }

        public void AddModifierProvider(ModifierProvider modifierProvider)
        {
            _modifiers.Add(modifierProvider);    
        }

        public void AddModifierProviders(List<ModifierProvider> modifierProviders)
        {
            _modifiers.AddRange(modifierProviders);
        }

        public void ClearModifiers() => _modifiers.Clear();
        
        public void ApplyAllModifiers(HeroView view)
        {
            foreach (var mod in _modifiers)
            {
                CLog.LogWhite($"[{view.gameObject.name}] Adding Spell: {mod.name}");
                mod.AddToHero(view);
            }
        }
        
    }
}