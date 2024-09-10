#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace Bomber
{
    [CustomEditor(typeof(MapBuilder))]
    public class MapBuilderEditor : Editor
    {
        private MapBuilder _me;
        
        private void OnEnable()
        {
            _me = target as MapBuilder;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(400);
            if (_me.ShowEditor)
            {
            }
        }
        
        
    }
}
#endif