#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(LabelsExample))]
    public class LabelsExampleEditor : Editor
    {
        private LabelsExample me;

        private void OnEnable()
        {
            me = target as LabelsExample;
        }

        public override void OnInspectorGUI()
        {
            EU.Label("Normal Label Left, Not Bold", 'l', false);
            EU.Label("Big Label Left, Not Bold",  EU.font_size_big,'l', false);
            EU.Label("Large Label Left, Not Bold",  EU.font_size_large,'l', false);
            EU.Label("Huge Label Left, Not Bold",  EU.font_size_huge,'l', false);
            EU.Space(EU.space_mid);
            
            EU.Label("Label Center, Bold", 'c', true);
            EU.Label("Label Center, Bold",  EU.font_size_big, 'c', true);
            EU.Label("Label Center, Bold",  EU.font_size_large,'c', true);
            EU.Space(EU.space_mid);
            
            EU.Label("Label Center, Bold",  Color.blue, 'c', true);
            EU.Label("Label Center, Bold", Color.cyan,  EU.font_size_big, 'c', true);
            EU.Label("Label Center, Bold", Color.yellow,  EU.font_size_large,'c', true);
            EU.Space(EU.space_mid);

            EU.Label("Label Right, Not Bold",  Color.blue, 'r', true);
            EU.Label("Label Right, Not Bold", Color.cyan,  EU.font_size_big, 'r', true);
            EU.Label("Label Right, Not Bold", Color.yellow,  EU.font_size_large,'r', true);
        }
    }
}
#endif