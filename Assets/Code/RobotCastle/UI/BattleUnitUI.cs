using SleepDev;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BattleUnitUI : MonoBehaviour
    {
        [SerializeField] private UnitUILayout _unitUILayout;
        [SerializeField] private StarsLevelView _levelView;

        public StarsLevelView Level => _levelView;
        
        public void SetMergeMode()
        {
            _unitUILayout.SetMerge();
        }

        public void SetBattleMode()
        {
            _unitUILayout.SetBattle();
        }
    }
}