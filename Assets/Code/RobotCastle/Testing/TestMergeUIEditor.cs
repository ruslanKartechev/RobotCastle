#if UNITY_EDITOR
using SleepDev;
using UnityEditor;

namespace RobotCastle.Testing
{
    [CustomEditor(typeof(TestMergeUI))]
    public class TestMergeUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as TestMergeUI;
            if (EU.BtnMidWide2("Not Enough Space", EU.Lime))
                me.ShowSpace();
            if (EU.BtnMidWide2("Not Troop Size", EU.Lime))
                me.ShowTroopsSize();
            if (EU.BtnMidWide2("No Money", EU.Lime))
                me.ShowMoney();
        }
    }
}
#endif