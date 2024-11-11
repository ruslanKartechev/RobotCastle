#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.DevCheat
{
    [CustomEditor(typeof(CheatKill))]
    public class CheatKillEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            var me = target as CheatKill;
            if (EU.BtnMid2("Kill", EU.Gold))
            {
                me.Kill();            
            }
            if (EU.BtnMid2("Kill List", EU.Gold))
            {
                me.KillList();            
            }

        }
    }
}
#endif