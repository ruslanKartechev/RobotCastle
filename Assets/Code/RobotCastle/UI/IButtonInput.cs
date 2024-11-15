using System;

namespace RobotCastle.UI
{
    public interface IButtonInput
    {
        /// <summary>
        /// Will add new callback, if it is not already in the list
        /// </summary>
        void AddMainCallback(Action callback);

        void RemoveMainCallback(Action callback);
        /// <summary>
        /// Will clean callbacks list and then add the new one
        /// </summary>
        void OverrideMainCallback(Action callback);

        void SetInteractable(bool interactable, bool visually = true);
        void On();
        void Off();
    }
}