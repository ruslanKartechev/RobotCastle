using UnityEngine;

namespace SleepDev
{
    public interface IPlayerInput
    {
        bool IsDown();
        bool IsUp();
        bool IsPressed();
        
        Vector3 MousePosition();
        void Enable();
        void Disable();
        bool IsActive { get; }
    }
}