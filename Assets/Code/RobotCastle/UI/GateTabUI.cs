using RobotCastle.Battling.Altars;
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
        [SerializeField] private MyButton _altarsBTN;

        private void Start()
        {
            _summonBtn.AddMainCallback(OpenSummonMenu);
            _battleBtn.AddMainCallback(OpenGameModeMenu);
            _settingBtn.AddMainCallback(OpenSettings);
            _altarsBTN.AddMainCallback(AltarsBtn);
            _summonBtn.SetInteractable(true);
            _battleBtn.SetInteractable(true);
            _settingBtn.SetInteractable(true);
        }

        private void AltarsBtn()
        {
            CLog.Log($"SHOW altars");
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ServiceLocator.Get<IUIManager>().Show<AltarsUI>(UIConstants.UIAltars, () =>
            {
                ServiceLocator.Get<TabsSwitcher>().SetGateTab();
            });
        }

        private void OpenSettings()
        {
            CLog.Log($"Open settings");
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ServiceLocator.Get<IUIManager>().Show<SettingsUI>(UIConstants.UISettings, () => 
            { 
                ServiceLocator.Get<TabsSwitcher>().SetGateTab();
            }).Show();
        }

        private void OpenSummonMenu()
        {
            CLog.Log($"Open summon menu");
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ServiceLocator.Get<IUIManager>().Show<SummoningUI>(UIConstants.UISummon, () => 
            { 
                ServiceLocator.Get<TabsSwitcher>().SetGateTab();
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