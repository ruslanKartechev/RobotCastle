#if UNITY_EDITOR
using UnityEditor;

namespace SleepDev
{
    [CustomEditor(typeof(SimplePopElement)), CanEditMultipleObjects]
    public class SimplePopElementEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
        }
    }
}
#endif