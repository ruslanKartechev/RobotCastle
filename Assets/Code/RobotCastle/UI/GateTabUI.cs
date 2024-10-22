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
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ServiceLocator.Get<IUIManager>().Show<AltarsOverviewUI>(UIConstants.UIAltars, () =>
            {
                ServiceLocator.Get<TabsSwitcher>().SetGateTab();
            }).Show();
        }

        private void OpenSettings()
        {
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ServiceLocator.Get<IUIManager>().Show<SettingsUI>(UIConstants.UISettings, () => 
            { 
                ServiceLocator.Get<TabsSwitcher>().SetGateTab();
            }).Show();
        }

        private void OpenSummonMenu()
        {
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ServiceLocator.Get<IUIManager>().Show<SummoningUI>(UIConstants.UISummon, () => 
            { 
                ServiceLocator.Get<TabsSwitcher>().SetGateTab();
            }).Show();
        }

        private void OpenGameModeMenu()
        {
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