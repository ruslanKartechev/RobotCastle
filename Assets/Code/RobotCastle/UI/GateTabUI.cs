using SleepDev;
using UnityEngine;

namespace RobotCastle.UI
{
    public class GateTabUI : MonoBehaviour, IScreenUI
    {

        public MyButton summonBtn => _summonBtn;
   
        public MyButton battleBtn => _battleBtn;

        public MyButton settingBtn => _settingBtn;

        [SerializeField] private MyButton _summonBtn;
        [SerializeField] private MyButton _battleBtn;
        [SerializeField] private MyButton _settingBtn;

        private void Start()
        {
            _summonBtn.AddMainCallback(OpenSummonMenu);
            _battleBtn.AddMainCallback(StartBattle);
            _settingBtn.AddMainCallback(OpenSettings);
            _summonBtn.SetInteractable(true);
            _battleBtn.SetInteractable(true);
            _settingBtn.SetInteractable(true);
        }

        private void OpenSettings()
        {
            CLog.Log($"Open settings");
        }

        private void OpenSummonMenu()
        {
            CLog.Log($"Open summon menu");
        }

        private void StartBattle()
        {
            CLog.Log($"Start battle call");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}