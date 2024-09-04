using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class TestMergeUI : MonoBehaviour
    {
        [SerializeField] private bool _autoSpawn = true;
        [SerializeField] private MergeInfoUI _ui;

        private void Start()
        {
            if(_autoSpawn)
                Spawn();
        }

        public void Spawn()
        {
            if (_ui != null)
            {
                CLog.Log($"Already spawned {_ui.gameObject.name}");
            }
            else
                _ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            _ui.ShowIdle();
        }

        public void ShowTroopsSize()
        {
            _ui.ShowNotEnoughTroopSize(3,3);
        }

        public void ShowSpace()
        {
            _ui.ShowNotEnoughSpace();
        }

        public void ShowMoney()
        {
            _ui.ShowNotEnoughMoney();
        }
    }
}