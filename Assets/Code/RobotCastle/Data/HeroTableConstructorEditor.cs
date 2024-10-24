#if UNITY_EDITOR
using SleepDev;
using UnityEditor;

namespace RobotCastle.Data
{
    [CustomEditor(typeof(HeroTableConstructor))]
    public class HeroTableConstructorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as HeroTableConstructor;
            EU.Space();
            if(EU.BtnMidWide2("Make Hero", EU.DarkGreen))
                me.MakeHero();
            
            EU.Space();
            if(EU.BtnMidWide2("Make Enemy", EU.Red))
                me.MakeEnemy();

            EU.Space();
            if(EU.BtnMidWide2("Make Manual Enemy", EU.Red))
                me.MakeEnemyFromScratch();
        }
    }
}
#endif