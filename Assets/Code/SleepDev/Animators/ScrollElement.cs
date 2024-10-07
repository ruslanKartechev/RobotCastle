using UnityEngine;

namespace SleepDev
{
    public abstract class ScrollElement : MonoBehaviour
    {
        public abstract void Close(bool leftToRight);
        public abstract void Show(bool leftToRight);
        public abstract void On();
        public abstract void Off();
    }
}