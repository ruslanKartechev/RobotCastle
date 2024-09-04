#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Testing
{
    [CustomEditor(typeof(TestBattleGridSpawner))]
    public class TestBattleGridSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            var me = target as TestBattleGridSpawner;
            if (EU.BtnMidWide2("+ Spawn Hero", EU.Plum))
                me.SpawnOneHero();
            if (EU.BtnMidWide2("+ Random Hero", EU.Gold))
                me.SpawnRandomHero();

            GUILayout.Space(10);
            if (EU.BtnMidWide2("+ Spawn Item", EU.Plum))
                me.SpawnOneItem();
            if (EU.BtnMidWide2("+ Random Item", EU.Gold))
                me.SpawnRandomItem();
            
        }
    }
}
#endif