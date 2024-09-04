using UnityEngine;

namespace SleepDev
{
    public class VerticalRotator : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Transform _target;

        private void Update()
        {
            var angles = _target.localEulerAngles;
            angles.y += _speed * Time.deltaTime;
            _target.localEulerAngles = angles;
        }
    }
}