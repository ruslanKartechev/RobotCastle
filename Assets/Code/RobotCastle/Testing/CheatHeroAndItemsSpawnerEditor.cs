#if UNITY_EDITOR
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RobotCastle.Testing
{
    [CustomEditor(typeof(CheatHeroAndItemsSpawner))]
    public class CheatHeroAndItemsSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(15);
            var me = target as CheatHeroAndItemsSpawner;
            GUILayout.BeginHorizontal();
            
            GUILayout.BeginVertical();
            if (EU.BtnMidWide("+ Hero", EU.Gold))
                me.SpawnOneHero();
            GUILayout.Space(5);
            if (EU.BtnMidWide("+ Item", EU.Gold))
                me.SpawnOneItem();
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
            if (EU.BtnMidWide("+ Preset 1", EU.Lime))
                me.SpawnPreset1();
            if (EU.BtnMidWide("+ Preset 2", EU.Lime))
                me.SpawnPreset2();
            if (EU.BtnMidWide("+ Preset 3", EU.Lime))
                me.SpawnPreset3();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);      
            
            // GUILayout.BeginHorizontal();

            EU.TwoButtonAndLabel("<<", ">>", $"Hero: {me.HeroID}", EU.Lime, EU.Lime, EU.White,
                        me.PrevHero, me.NextHero);
            GUILayout.Space(5);
            EU.TwoButtonAndLabel("<<", ">>", $"Level: {me.HeroLvl+1}", EU.Lime, EU.Lime, EU.White,
                        me.PrevHeroLvl, me.NextHeroLvl);
            GUILayout.Space(5);
            if(EU.BtnMidWide2($"Spawn {me.HeroID}", EU.Fuchsia))
                me.SpawnChosenHero();
            
            // GUILayout.EndHorizontal();
            
            
            GUILayout.Space(10);      

            
            GUILayout.BeginHorizontal();
            if (EU.BtnMid1("Sword 1", EU.Red))
                me.SpawnSword1();
            if (EU.BtnMid1("Sword 2", EU.Red))
                me.SpawnSword2();
            if (EU.BtnMid1("Sword 3", EU.Red))
                me.SpawnSword3();
            if (EU.BtnMid1("Sword 4", EU.Red))
                me.SpawnSword4();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);      

            GUILayout.BeginHorizontal();
            if (EU.BtnMid1("Staff 1", EU.Aqua))
                me.SpawnStaff1();
            if (EU.BtnMid1("Staff 2", EU.Aqua))
                me.SpawnStaff2();
            if (EU.BtnMid1("Staff 3", EU.Aqua))
                me.SpawnStaff3();
            if (EU.BtnMid1("Staff 4", EU.Aqua))
                me.SpawnStaff4();
            GUILayout.EndHorizontal();
         
            GUILayout.Space(5);      

            GUILayout.BeginHorizontal();
            if (EU.BtnMid1("Armor 1", EU.Gold))
                me.SpawnArmor1();
            if (EU.BtnMid1("Armor 2", EU.Gold))
                me.SpawnArmor2();
            if (EU.BtnMid1("Armor 3", EU.Gold))
                me.SpawnArmor3();
            if (EU.BtnMid1("Armor 4", EU.Gold))
                me.SpawnArmor4();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);      

            GUILayout.BeginHorizontal();
            if (EU.BtnMid1("Bow 1", EU.Fuchsia))
                me.SpawnBow1();
            if (EU.BtnMid1("Bow 2", EU.Fuchsia))
                me.SpawnBow2();
            if (EU.BtnMid1("Bow 3", EU.Fuchsia))
                me.SpawnBow3();
            if (EU.BtnMid1("Bow 4", EU.Fuchsia))
                me.SpawnBow4();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);      

            GUILayout.BeginHorizontal();
            if (EU.BtnMid2("XP book 1", EU.Lavender))
                me.SpawnXPBook1();
            if (EU.BtnMid2("XP book 2", EU.Lavender))
                me.SpawnXPBook2();
            if (EU.BtnMid2("XP book 3", EU.Lavender))
                me.SpawnXPBook3();
            GUILayout.EndHorizontal();
        }
    }
}
#endif