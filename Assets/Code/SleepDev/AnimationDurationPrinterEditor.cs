#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(AnimationDurationPrinter))]
    public class AnimationDurationPrinterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as AnimationDurationPrinter;
            if (GUILayout.Button("Print Length all", GUILayout.Width(120)))
            {
                me.PrintAll();
            }
        }
    }
}
#endif