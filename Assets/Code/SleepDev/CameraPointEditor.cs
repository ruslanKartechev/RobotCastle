#if UNITY_EDITOR
using UnityEditor;

namespace SleepDev
{
    [CustomEditor(typeof(CameraPoint))]
    public class CameraPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as CameraPoint;
            if(me.AutoSet)
            {
                CameraPointMover.SetToPoint(me);
            }
            
        }
    }
}
#endif