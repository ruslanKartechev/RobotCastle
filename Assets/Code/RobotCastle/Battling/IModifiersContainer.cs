using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public interface IModifiersContainer
    {
        ModifierProvider defaultSpell { get; }
        ModifierProvider GetCurrentSpell();
        void AddProvider(ModifierProvider modifierProvider);
        void AddProviders(List<ModifierProvider> modifierProviders);
        void ClearAll();
        void ApplyAll(HeroComponents view);
    }
}