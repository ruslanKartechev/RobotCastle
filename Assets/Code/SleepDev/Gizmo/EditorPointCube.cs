using UnityEngine;

namespace SleepDev
{
    public class EditorPointCube : MonoBehaviour
    {
#if UNITY_EDITOR
         public bool doDraw = true;
         public Color color = Color.green;
         public float radius;

        
        private void OnDrawGizmos()
        {
            if (!doDraw)
                return;
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position, Vector3.one * (radius / 2f));
            Gizmos.color = oldColor;
        }

#endif
    }
}