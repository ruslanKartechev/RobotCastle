using UnityEngine;

namespace SleepDev.Ragdoll
{
    public abstract class IRagdoll : MonoBehaviour
    {
        public abstract void Deactivate();
        public abstract void Activate();
        public abstract void ActivateAndPush(Vector3 force);
        public abstract void ZeroVelocities();
        public abstract void SetGravityAll(bool on);
        public abstract void PushPartsFromOrigin(float force);
        public abstract bool IsActive { get; protected set; }
    }
}