#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Saving
{
    [CustomEditor(typeof(DataInitializer))]
    public class DataInitializerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as DataInitializer;
            GUILayout.Space(10);
            if (EU.BtnMid2("Clear", EU.Red))
                me.ClearAllSaveFiles();
            if (EU.BtnMid2("Path", EU.Aqua))
                me.LogSavesPath();
        }
    }
}
#endif