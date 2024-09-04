using System;
using UnityEngine;

namespace SleepDev
{
    public class HitTriggerReceiver : MonoBehaviour
    {
        [SerializeField] private Collider _collider;

        public Collider Collider => _collider;
        public Action<Collider> Callback { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            Callback?.Invoke(other);
        }
    }
}