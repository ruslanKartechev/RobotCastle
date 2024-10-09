#if UNITY_EDITOR
using SleepDev;
using UnityEditor;

namespace RobotCastle.Battling
{
    [CustomEditor(typeof(BattleLevel))]
    public class BattleLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BattleLevel;
            EU.Space();
            if (EU.BtnMidWide2("Force Smelting", EU.Orange))
                me.ForceShowSmeltingOffer();
            if (EU.BtnMidWide2("Force Devils", EU.Orange))
                me.ForceShowDevilOffer();
            
            if (EU.BtnMidWide2("Force Trader", EU.Orange))
                me.ForceShowTradeOffer();

            if (EU.BtnMidWide2("Force Win", EU.Lime))
                me.ForceWin();
            if (EU.BtnMidWide2("Force Fail", EU.Lime))
                me.ForceFail();
            
        }        
    }
}
#endif