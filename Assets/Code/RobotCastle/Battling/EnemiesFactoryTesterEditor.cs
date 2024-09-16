#if UNITY_EDITOR

using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CustomEditor(typeof(EnemiesFactoryTester))]
    public class EnemiesFactoryTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as EnemiesFactoryTester;
            GUILayout.Space(10);
            
            if (EU.BtnMidWide3("Spawn TestPack", EU.Gold))
            {
                me.SpawnTestPack();
            }
        }
    }
}
#endif