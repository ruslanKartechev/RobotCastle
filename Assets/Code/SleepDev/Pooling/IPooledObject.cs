using UnityEngine;

namespace SleepDev
{
    public interface IPooledObject<T> where T : class
    {
        IObjectPool<T> Pool { get; set; }
        void Parent(Transform parent);
        T Obj { get; }
        void Destroy();
        void Hide();
    }
}