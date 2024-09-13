using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public interface IModifiersContainer
    {
        List<ModifierProvider> modifiers { get; }
        ModifierProvider defaultSpell { get; }
        ModifierProvider currentSpell { get; }
    }
}