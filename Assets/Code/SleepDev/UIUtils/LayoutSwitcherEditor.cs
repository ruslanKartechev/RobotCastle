#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(LayoutSwitcher))]
    public class LayoutSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as LayoutSwitcher;
            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide("Prev", Color.cyan))
                me.SetPrevLayout();
            if (EU.BtnMidWide("Next", Color.cyan))
                me.SetNextLayout();   
            GUILayout.EndHorizontal();
        }
    }
}
#endif