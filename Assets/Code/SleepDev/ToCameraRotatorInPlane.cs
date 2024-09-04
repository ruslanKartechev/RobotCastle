using UnityEngine;

namespace SleepDev
{
    public class ToCameraRotatorInPlane : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        private Transform _cameraTr;
        
        private void OnEnable()
        {
            _cameraTr = Camera.main.transform;
        }

        private void Update()
        {
            var lookVec = (_target.position - _cameraTr.position);
            // lookVec.y = 0;
            _target.rotation = Quaternion.LookRotation(lookVec);
            var angles = _target.eulerAngles;
            angles.z = angles.x = 0;
            _target.eulerAngles = angles;
        }
        
    }
}