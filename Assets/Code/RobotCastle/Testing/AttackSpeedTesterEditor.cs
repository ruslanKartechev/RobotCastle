#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Testing
{
    [CustomEditor(typeof(AttackSpeedTester))]
    public class AttackSpeedTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as AttackSpeedTester;
            GUILayout.Space(15);
            if(EU.BtnMidWide("Spawn", EU.Gold))
                me.Spawn();
            GUILayout.Space(5);
            if(EU.BtnMidWide("Attack", EU.Gold))
                me.StartAttack();
            
            GUILayout.Space(15);
            if(EU.BtnMidWide("Delete All", EU.Red))
                me.DeleteAll();
            
            GUILayout.Space(15);
            if(EU.BtnMidWide3("Save Animation Speed", EU.DarkCyan))
                me.RecordAllAverageSpeed();
            if(EU.BtnMidWide3("Set recorded speed", EU.DarkCyan))
                me.SetAnimationSpeedFromSave();
        }
    }
}
#endif