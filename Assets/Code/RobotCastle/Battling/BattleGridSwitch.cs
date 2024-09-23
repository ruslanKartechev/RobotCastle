using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleGridSwitch : MonoBehaviour
    {
        [SerializeField] private GameObject _playerGrid;
        [SerializeField] private GameObject _enemyGrid;

        public void SetBattleMode()
        {
            _playerGrid.SetActive(false);
            _enemyGrid.SetActive(false);
        }

        public void SetMergeMode()
        {
            _playerGrid.SetActive(true);
            _enemyGrid.SetActive(true);
        }
        
    }
}