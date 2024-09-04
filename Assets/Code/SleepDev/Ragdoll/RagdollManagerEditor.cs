#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev.Ragdoll
{
    [CustomEditor(typeof(Ragdoll))]
    public class RagdollManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var me = target as Ragdoll;

            EU.Label("Ragdoll", Color.white, 'c', true);
            GUILayout.BeginHorizontal();
            if (EU.BtnMid1("Get", Color.cyan))
            {
                me.E_GetParts();
                EditorUtility.SetDirty(me);
            }
            if (EU.BtnMid1("On", Color.green))
            {
                 me.Activate();   
                 EditorUtility.SetDirty(me);
            }
            if (EU.BtnMid1("Off", Color.red))
            {
                me.Deactivate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.BtnMid1("Colls", Color.yellow))
            {
                me.SetCollidersOnly();
                EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space((10));
            
            EU.Label("Interpolate", Color.white, 'l', true);
            GUILayout.BeginHorizontal();
            if (EU.BtnSmallSquare("I", Color.green))
            {
                me.E_SetInterpolate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.BtnSmallSquare("E", Color.yellow))
            {
                me.E_SetExtrapolate();   
                EditorUtility.SetDirty(me);
            }
            if (EU.BtnSmallSquare("N", Color.red))
            {
                me.E_SetNoInterpolate();   
                EditorUtility.SetDirty(me);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space((10));

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                EU.Label("Projection", Color.white, 'l', true);
                GUILayout.BeginHorizontal();
                if (EU.BtnSmallSquare("Y", Color.green))
                {
                    me.E_SetProjection();   
                    EditorUtility.SetDirty(me);
                }       
                if (EU.BtnSmallSquare("N", Color.red))
                {
                    me.E_SetNoProjection();   
                    EditorUtility.SetDirty(me);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                
                GUILayout.BeginVertical();
                EU.Label("Preprocessing", Color.white, 'l', true);
                GUILayout.BeginHorizontal();
                if (EU.BtnSmallSquare("Y", Color.green))
                {
                    me.E_SetPreprocessAll(true);
                    EditorUtility.SetDirty(me);
                }       
                if (EU.BtnSmallSquare("N", Color.red))
                {
                    me.E_SetPreprocessAll(false);
                    EditorUtility.SetDirty(me);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide($"Set Layer {me.E_layerToSet}", Color.green))
                me.SetLayer();
            if (EU.BtnMidWide($"Set Mass", Color.yellow))
                me.E_SetMassAll();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide($"Copy", Color.blue))
                me.E_Copy();
            GUILayout.Space(30);
            if (EU.BtnMidWide($"! DELETE !", Color.red))
                me.E_DestroyAll();
            GUILayout.EndHorizontal();
            
            GUILayout.Space((10));
            base.OnInspectorGUI();
            
       
        }
    }
}
#endif