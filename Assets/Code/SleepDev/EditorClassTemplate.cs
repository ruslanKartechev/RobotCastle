using UnityEngine;
using UnityEditor;

namespace SleepDev
{
#if UNITY_EDITOR
    public class TemplateClass : MonoBehaviour{}
#endif

    
    
    
#if UNITY_EDITOR
    [CustomEditor(typeof(TemplateClass))]
    public class TemplateClassEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as TemplateClass;
        }
    }
#endif
}