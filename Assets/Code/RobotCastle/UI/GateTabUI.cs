using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.MainMenu;
using RobotCastle.Summoning;
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
            _battleBtn.AddMainCallback(OpenGameModeMenu);
            _settingBtn.AddMainCallback(OpenSettings);
            _summonBtn.SetInteractable(true);
            _battleBtn.SetInteractable(true);
            _settingBtn.SetInteractable(true);
        }

        private void OpenSettings()
        {
            CLog.Log($"Open settings");
            gameObject.SetActive(false);
            ServiceLocator.Get<IUIManager>().Show<SettingsUI>(UIConstants.UISettings, () => 
            { 
                gameObject.SetActive(true);
            }).Show();
        }

        private void OpenSummonMenu()
        {
            CLog.Log($"Open summon menu");
            gameObject.SetActive(false);
            ServiceLocator.Get<IUIManager>().Show<SummoningUI>(UIConstants.UISummon, () => 
            { 
                gameObject.SetActive(true);
            }).Show();
        }

        private void OpenGameModeMenu()
        {
            CLog.Log($"On battle button");
            // gameObject.SetActive(false);
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ServiceLocator.Get<IUIManager>().Show<GameModeSelectionUI>(UIConstants.UIGameModeSelection, () => 
            { 
                ServiceLocator.Get<TabsSwitcher>().SetGateTab();
            }).Show();
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