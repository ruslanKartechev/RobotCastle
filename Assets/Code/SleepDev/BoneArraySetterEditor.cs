#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(BoneArraySetter))]
    public class BoneArraySetterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BoneArraySetter;
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Print from", GUILayout.Width(100)))
            {
                me.PrintFromBones();
                Dirty();
            }
            if(GUILayout.Button("Print To", GUILayout.Width(100)))
            {
                me.PrintToBones();
                Dirty();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Copy Bones", GUILayout.Width(100)))
            {
                me.CopyBones();
                Dirty();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Get bones", GUILayout.Width(100)))
            {
                me.GetBones();
                Dirty();
            }
            if(GUILayout.Button("Get names", GUILayout.Width(100)))
            {
                me.GetBonesNames();
                Dirty();
            }
            if(GUILayout.Button("Set copied", GUILayout.Width(100)))
            {
                me.FindAndSetBones();
                Dirty();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Get copy_to by names", GUILayout.Width(130)))
            {
                me.GetCopyToByNames();   
                Dirty();
            }
            if(GUILayout.Button("Get copy_to by refs", GUILayout.Width(130)))
            {
                me.GetCopyToByRefs();   
                Dirty();
            }
            GUILayout.EndHorizontal();
            
            if(GUILayout.Button("Check Names Mismatch", GUILayout.Width(130)))
            {
                me.CheckNamesMismatch();   
                Dirty();
            }
            
            void Dirty()
            {
                UnityEditor.EditorUtility.SetDirty(me);
            }
        }
    }
}
#endif