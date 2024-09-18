#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Battling
{
    [CustomEditor(typeof(BattleManager))]
    public class BattleManagerEditor : Editor
    {
        public override void OnInspectorGUI() 
        {
            base.OnInspectorGUI();
            var me = target as BattleManager;
            GUILayout.Space(15);
            
            if(EU.BtnMidWide("Print State", EU.Aqua))
                me.PrintState();
            if(EU.BtnMidWide("Print Player", EU.Gold))
                me.PrintPlayerHeroes();
            if(EU.BtnMidWide("Print Enemies", EU.Orange))
                me.PrintEnemyHeroes();
            GUILayout.Space(15);
            if(EU.BtnMidWide("Begin Battle", EU.GreenYellow))
                me.BeginBattle();
        }
    }
}
#endif