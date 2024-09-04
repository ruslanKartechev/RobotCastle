using UnityEngine;

namespace SleepDev
{
    public class ZRotator : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Transform _target;

        private void Update()
        {
            var angles = _target.eulerAngles;
            angles.z += _speed * Time.deltaTime;
            _target.eulerAngles = angles;
        }
    }
}