using UnityEngine;

namespace SleepDev.Ragdoll
{
    public partial class Ragdoll
    {
        [System.Serializable]
        public class RagdollPart
        {
            public Collider collider;
            public Rigidbody rb;
            public bool push;
            public string name;

            public void Off()
            {
                rb.isKinematic = true;
                collider.enabled = false;
            }

            public void On()
            {
                rb.isKinematic = false;
                collider.enabled = true;
            }

            public void Push(Vector3 force)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}