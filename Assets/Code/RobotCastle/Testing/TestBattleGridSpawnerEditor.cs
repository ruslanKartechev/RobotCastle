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
            GUILayout.Space(15);
            var me = target as TestBattleGridSpawner;
            if (EU.BtnMidWide2("+ Spawn Hero", EU.Gold))
                me.SpawnOneHero();
            if (EU.BtnMidWide2("+ Random Hero", EU.Gold))
                me.SpawnRandomHero();

            GUILayout.Space(10);
            if (EU.BtnMidWide2("+ Spawn Item", EU.Lavender))
                me.SpawnOneItem();
            if (EU.BtnMidWide2("+ Random Item", EU.Lavender))
                me.SpawnRandomItem();
            
            GUILayout.Space(10);
            if (EU.BtnMidWide2("+ Spawn List", EU.Lime))
                me.SpawnItemsList();
        }
    }
}
#endif