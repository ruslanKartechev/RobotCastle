using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public interface IModifiersContainer
    {
        ModifierProvider defaultSpell { get; }
        ModifierProvider GetCurrentSpell();
        void AddModifierProvider(ModifierProvider modifierProvider);
        void AddModifierProviders(List<ModifierProvider> modifierProviders);
        void ClearModifiers();
        void ApplyAllModifiers(HeroView view);
    }
}