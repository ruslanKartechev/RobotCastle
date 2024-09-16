#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CustomEditor(typeof(EnemiesFactory))]
    public class EnemiesFactoryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            var me = target as EnemiesFactory;
            
            if(EU.BtnMidWide3("Make Test Preset", EU.Aqua))
                me.E_CreateTestPreset();
            
        }
    }
}
#endif