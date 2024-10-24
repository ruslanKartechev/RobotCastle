#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Utils
{
    [CustomEditor(typeof(SpringAnimator))]
    public class SpringAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as SpringAnimator;
            EU.Space();
            EU.Space();
            
            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide("Begin From Start", EU.Gold))
                me.BeginAnimatingFromStart();
            GUILayout.Space(4);
            if (EU.BtnMid2("Stop", EU.Purple))
                me.Stop();
            GUILayout.EndHorizontal();
        }
    }
}
#endif