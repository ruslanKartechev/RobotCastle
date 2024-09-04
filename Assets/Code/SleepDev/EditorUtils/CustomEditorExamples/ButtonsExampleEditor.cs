// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEngine;
//
// namespace SleepDev.CustomEditorExamples
// {
//     [CustomEditor(typeof(ButtonsExample))]
//     public class ButtonsExampleEditor : Editor
//     {
//         private ButtonsExample me;
//
//         private void OnEnable()
//         {
//             me = target as ButtonsExample;
//         }
//
//         public override void OnInspectorGUI()
//         {
//             var click_me = "click me";
//             var here_too = "here too";
//             
//             GUILayout.BeginHorizontal();
//             if (EU.ButtonSmall("B", Color.blue))
//                 Debug.Log("Small button 1 clicked");
//             if (EU.ButtonSmall("T", Color.green))
//                 Debug.Log("Small button 2 clicked");
//             if (EU.ButtonSmall("T", Color.yellow))
//                 Debug.Log("Small button 3 clicked");
//             GUILayout.EndHorizontal();
//             
//             if (EU.ButtonMidSize(click_me, Color.cyan))
//                 Debug.Log("Mid Size button clicked");
//
//             GUILayout.BeginHorizontal();
//             if (EU.ButtonBig(click_me, Color.green))
//                 Debug.Log("Big button clicked");
//             if (EU.ButtonLarge(click_me, Color.yellow))
//                 Debug.Log("Large button clicked");
//             GUILayout.EndHorizontal();
//
//             
//             GUILayout.BeginHorizontal();
//             if (EU.ButtonBigWide(click_me + "\n" + here_too, Color.red))
//                 Debug.Log("Big Wide button clicked");
//             if (EU.ButtonLargeWide(click_me + "\n" + here_too, Color.magenta))
//                 Debug.Log("Large wide clicked");
//             GUILayout.EndHorizontal();
//         }
//     }
// }
// #endif