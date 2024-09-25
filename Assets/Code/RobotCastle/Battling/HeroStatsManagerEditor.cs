#if UNITY_EDITOR
using SleepDev;
using UnityEditor;

namespace RobotCastle.Battling
{
    [CustomEditor(typeof(HeroStatsManager))]
    public class HeroStatsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as HeroStatsManager;
            if(EU.BtnMidWide3("Log All Decorators", EU.Aqua))
                me.LogAllDecorators();
        }
    }
}
#endif