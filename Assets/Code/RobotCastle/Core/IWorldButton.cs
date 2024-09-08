using System;

namespace RobotCastle.Core
{
    public interface IWorldButton
    {
        event Action OnClicked;
        void OnDown();
        void OnUp();
    }
}