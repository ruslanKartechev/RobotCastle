using UnityEngine;

namespace SleepDev
{
    public class OnGizmosSphereDrawer : OnGizmosDrawer
    {
#if UNITY_EDITOR
        [SerializeField] protected float _drawRadius;
        [SerializeField] protected Transform _drawAtPoint;

        public float drawRadius
        {
            get => _drawRadius;
            set => _drawRadius = value;
        }

        public Transform drawAtPoint
        {
            get => _drawAtPoint;
            set => _drawAtPoint = value;
        }

        public void DrawSphere()
        {
            DrawSphere(_drawAtPoint.position, _drawRadius);
        }
        
        public void DrawSphere(Vector3 at)
        {
            DrawSphere(at, _drawRadius);
        }

        public void DrawSphere(Vector3 at, float radius)
        {
            if(_doDraw ==false)
                return;
            var color = Gizmos.color;
            Gizmos.color = _color;
            Gizmos.DrawSphere(at, radius);
            Gizmos.color = color;
        }
#endif
    }
}