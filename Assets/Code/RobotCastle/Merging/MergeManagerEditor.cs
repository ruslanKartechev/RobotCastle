#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Merging
{
    [CustomEditor(typeof(MergeManager))]
    public class MergeManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as MergeManager;
            GUILayout.Space(10);
            if (EU.BtnMid2("Log", EU.Fuchsia))
            {
                me.LogGridState();
            }
        }
    }
}
#endif