using UnityEngine;

namespace SleepDev
{
    public abstract class PopElement : MonoBehaviour
    {
        public abstract float Delay { get; set; }
        public abstract float Duration { get; set; }

        public abstract void ScaleUp();
        public abstract void ScaleDown();
    }
}