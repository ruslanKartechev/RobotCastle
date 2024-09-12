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
            if (EU.BtnMidWide2("CreateForEveryId", EU.Gold))
            {
                me.CreateForEveryId();
            }
            if (EU.BtnMidWide3("CreateForEveryId Forces", EU.Gold))
            {
                me.CreateForEveryIdForced();
            }

        }
    }
}
#endif