// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEngine;
//
// namespace SleepDev.Utils.EditorUtils.CustomEditorExamples
// {
//     [CustomEditor(typeof(GUIWindowExample))]
//     public class GUIWindowExampleEditor : Editor
//     {
//         private GUIWindowExample me;
//         private Rect windowRect0 = new Rect(20, 20, 120, 50);
//         
//         
//         private void OnEnable()
//         {
//             me = target as GUIWindowExample;
//         }
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             // GUI.Window(0, windowRect0, WindowFunction, "My window");
//             if (EU.ButtonBig("Open", EU.DarkGreen))
//                 me.Open();
//             if (EU.ButtonBig("Close", EU.Pink))
//                 me.Close();
//         }
//
//
//         
//     }
// }
// #endif