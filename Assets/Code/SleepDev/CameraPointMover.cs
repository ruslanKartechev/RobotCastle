using UnityEngine;

namespace SleepDev
{
    public class CameraPointMover : MonoBehaviour
    {
        private static CameraPointMover _instance;

        public static void SetToPoint(ICameraPoint point) => _instance.CamSetToPoint(point);
        public static void ParentToPoint(ICameraPoint point, bool center = true) => _instance.CamParentToPoint(point, center);
        
        
        [SerializeField] private Transform _movable;

        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            _instance = this;
        }
        #endif

        private void OnEnable()
        {
            _instance = this;
        }

        private void CamSetToPoint(ICameraPoint point)
        {
            var pp = point.GetPoint();
            _movable.SetPositionAndRotation(pp.position, pp.rotation);
        }

        private void CamParentToPoint(ICameraPoint point, bool center = true)
        {
            var pp = point.GetPoint();
            _movable.parent = pp;
            if(center)
                _movable.SetPositionAndRotation(pp.position, pp.rotation);
        }
    }
}