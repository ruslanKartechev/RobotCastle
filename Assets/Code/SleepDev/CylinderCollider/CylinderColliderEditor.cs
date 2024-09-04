#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(CylinderCollider))]
    public class CylinderColliderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var me = target as CylinderCollider;
            base.OnInspectorGUI();
            GUILayout.Space(10); 
            if (EU.BtnMidWide3("Build And Assign", EU.Gold))
                me.BuildAndAssign();
            if (EU.BtnMidWide3("Log Mesh Data", EU.Lime))
                me.LogMeshData();
            
            GUILayout.Space(5);
            if (EU.BtnMidWide3("Set view components", EU.Aqua))
                me.TryAddViewComponents();
            
            GUILayout.Space(10);
            if (EU.BtnMidWide2("Build mesh", EU.Aqua))
                me.BuildNewMesh();
            if (EU.BtnMidWide2("Assign mesh", EU.Aqua))
                me.AssignMesh();

        }
    }
}
#endif