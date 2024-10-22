using RobotCastle.Core;

namespace RobotCastle.Battling
{
    public interface IKillProcessor : IModifiable<IKIllModifier>
    {
        void Kill();
    }
}