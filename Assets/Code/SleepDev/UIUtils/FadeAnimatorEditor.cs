#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(FadeAnimator))]
    public class FadeAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as FadeAnimator;
            if (GUILayout.Button("Get", GUILayout.Width(120)))
            {
                me.E_GetAll();
            }
            if (GUILayout.Button("Fade in", GUILayout.Width(120)))
            {
                me.E_FadeIn();
            }
            if (GUILayout.Button("Fade out", GUILayout.Width(120)))
            {
                me.E_FadeOut();
            }
            if (GUILayout.Button("Set Ease", GUILayout.Width(120)))
            {
                me.E_SetEaseAll();
            }
            if (GUILayout.Button("SetTime IN", GUILayout.Width(120)))
            {
                me.E_SetInTimeAll();
            }
            if (GUILayout.Button("SetTime OUT", GUILayout.Width(120)))
            {
                me.E_SetOutTimeAll();
            }


        }
    }
}
#endif