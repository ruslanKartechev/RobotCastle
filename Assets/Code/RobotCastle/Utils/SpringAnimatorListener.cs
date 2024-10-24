using UnityEngine;

namespace RobotCastle.Utils
{
    public abstract class SpringAnimatorListener : MonoBehaviour
    {
        public abstract void OnStarted();
        public abstract void OnUpdated(float val);
        public abstract void OnStopped();
    }
}