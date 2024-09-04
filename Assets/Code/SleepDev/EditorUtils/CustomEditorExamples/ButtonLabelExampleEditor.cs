#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    [CustomEditor(typeof(ButtonLabelExample))]
    public class ButtonLabelExampleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EU.TwoButtonAndLabel("<<", ">>", "Switch like to buttons Left",
                EU.Pink, EU.Fuchsia, EU.White,
                () =>{}, () => { });
            
            EU.LabelAndTwoButton("<<", ">>", "Switch like to buttons Right",
                EU.Fuchsia, EU.Lime, EU.White,
                () =>{}, () => { });
            
            EU.ThreeButtonAndLabel("<<", "U",">>", "Switch like to buttons Left",
                EU.Gold, EU.Orange, EU.Gold, Color.white,
                () =>{}, () => { }, () =>{});
            EU.Space(EU.space_big);
            
            if (EU.ButtonWithLabelSmall("B", "Label for the button", EU.Navy))
                Debug.Log("Pressed");
            if (EU.ButtonWithLabelMidSize("Y", "Label for the button", EU.Plum))
                Debug.Log("Pressed");
            if (EU.ButtonWithLabelBig("C", "Label line 1\nLabel line 2", EU.DarkCyan))
                Debug.Log("Pressed");
            if (EU.ButtonWithLabelLarge("R", "Line 1 \nLine2 \nLine3", EU.Violent))
                Debug.Log("Pressed");
            
            EU.Space(EU.space_big);
            if (EU.LabelWithButton("C", "Button with label left",
                    Color.cyan, Color.white,
                    EU.square_btn_size_mid)) ;
            
            if (EU.LabelWithButton("R", "Button with label left",
                    Color.red, Color.white,
                    EU.square_btn_size_mid, 15)) ;
        }
        
    }
}
#endif