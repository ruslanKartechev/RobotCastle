#if UNITY_EDITOR

using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Testing
{
    [CustomEditor(typeof(EnemiesTester))]
    public class EnemiesFactoryTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as EnemiesTester;
            GUILayout.Space(15);
            
            if (EU.BtnMidWide("Run", EU.Gold))
            {
                me.Run();
            }
        }
    }
}
#endif