#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(CameraShaker))]
    public class CameraShakerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me =target as CameraShaker;
            if (GUILayout.Button($"Shake", GUILayout.Width(100)))
            {
                me.PlayDefault();
            }
        }
    }
}
#endif