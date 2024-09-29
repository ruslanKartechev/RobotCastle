#if UNITY_EDITOR
using SleepDev;
using UnityEditor;

namespace RobotCastle.Battling
{
    [CustomEditor(typeof(HeroesDatabaseContainer))]
    public class HeroesDatabaseContainerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var me = target as HeroesDatabaseContainer;
            base.OnInspectorGUI();

            EU.Space();
            if (EU.BtnMidWide3($"Multiply {me.e_statType.ToString()} by {me.e_multiplier}", EU.Lime))
                me.MultiplyStatForChosenId();
        }
    }
}
#endif