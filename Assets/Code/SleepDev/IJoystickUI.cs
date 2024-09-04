using UnityEngine;

namespace SleepDev
{
    public interface IJoystickUI
    {
        void Show();
        void Hide();
        void SetPosition(Vector3 screenPosition);
        void SetJoystickLocal(Vector3 screenPosition);
        void SetJoystickLocal(Vector3 normalizedPos, float percent);
    }
}