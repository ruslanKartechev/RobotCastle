using UnityEngine;

namespace SleepDev
{
    public abstract class ButtonClickEffect : MonoBehaviour
    {
        [SerializeField] protected RectTransform _target;
        public abstract void Play();
    }
}