#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CustomEditor(typeof(BattleLevel))]
    public class BattleLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BattleLevel;
            GUILayout.Space(15);

            if (EU.BtnMidWide2("Force Smelting", EU.Orange))
                me.ForceShowSmeltingOffer();
            if (EU.BtnMidWide2("Force Devils", EU.Orange))
                me.ForceShowDevilOffer();
            if (EU.BtnMidWide2("Force Trader", EU.Orange))
                me.ForceShowTradeOffer();
            
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            if (EU.BtnMidWide2("Force Win", EU.Lime))
                me.ForceWin();
            if (EU.BtnMidWide2("Force Fail", EU.Lime))
                me.ForceFail();
            GUILayout.EndHorizontal();
            
        }        
    }
}
#endif