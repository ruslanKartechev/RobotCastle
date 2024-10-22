#if UNITY_EDITOR
using SleepDev;
using UnityEditor;

namespace RobotCastle.Data
{
    [CustomEditor(typeof(HeroesDatabaseContainer))]
    public class HeroesDatabaseContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var me = target as HeroesDatabaseContainer;
            base.OnInspectorGUI();

            EU.Space();
            EU.Space();
            EU.Space();
            if (EU.BtnMidWide3($"GetAllEnemyConfigFiles", EU.Gold))
                me.GetAllEnemyConfigFileNames();
            
            EU.Space();
            
            if (EU.BtnMidWide3($"Multiply {me.e_statType.ToString()} by {me.e_multiplier}", EU.Gold))
                me.MultiplyStatForChosenId();
        }
    }
}
#endif