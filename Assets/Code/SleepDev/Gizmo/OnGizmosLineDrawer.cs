using UnityEngine;

namespace SleepDev
{
    public class OnGizmosLineDrawer : OnGizmosDrawer
    {
#if UNITY_EDITOR
        [SerializeField] protected bool _doDrawSpheres;
        [SerializeField] protected float _radius;
        
        public void DrawLine(Vector3 from, Vector3 to)
        {
            if(_doDraw == false)
                return;
            var color = Gizmos.color;
            Gizmos.color = _color;
            Gizmos.DrawLine(from, to);
            if (_doDrawSpheres)
            {
                Gizmos.DrawSphere(from, _radius);
                Gizmos.DrawSphere(to, _radius);
            }
            Gizmos.color = color;
        }
#endif
    }
}