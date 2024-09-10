#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Testing
{
    [CustomEditor(typeof(UnitsMovementTester))]
    public class UnitsMovementTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(20);
            var me = target as UnitsMovementTester;
            if(EU.BtnMidWide2("Build Map", EU.Navy))
                me.BuildMap();
            GUILayout.Space(10);
            if(EU.BtnMidWide2("GetUnits", EU.Aqua))
                me.GetUnits();
            if(EU.BtnMidWide2("Move To Pos", EU.Gold))
                me.MoveOneToPos();

        }
    }
}
#endif