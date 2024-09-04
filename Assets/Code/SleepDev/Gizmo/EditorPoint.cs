using UnityEditor;
using UnityEngine;

namespace SleepDev
{
#if UNITY_EDITOR
    public class EditorPoint : MonoBehaviour
    {
        public bool doDraw = true;
        public Color color = Color.green;
        public float radius;
        [Space(10)]
        public bool drawDirection;
        public float directionLength = 1f;

        private void OnDrawGizmos()
        {
            if (!doDraw)
                return;
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
            if (drawDirection)
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * directionLength);
            Gizmos.color = oldColor;
        }
    }
#endif
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(EditorPoint)), CanEditMultipleObjects()]
    public class EditorPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    #endif
}