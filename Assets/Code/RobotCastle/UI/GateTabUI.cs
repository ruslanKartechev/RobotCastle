using RobotCastle.Battling.Altars;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.MainMenu;
using RobotCastle.Relics;
using RobotCastle.Summoning;
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
        [SerializeField] private MyButton _relicsBtn;
        [SerializeField] private SideMoveAnimator _sideMoveAnimator;    
        
        private void Start()
        {
            _summonBtn.AddMainCallback(OpenSummonMenu);
            _battleBtn.AddMainCallback(OpenGameModeMenu);
            _settingBtn.AddMainCallback(OpenSettings);
            _altarsBTN.AddMainCallback(AltarsBtn);
            _relicsBtn.AddMainCallback(RelicsBtn);
            SetBtnsInteractable(true);
        }

        private void RelicsBtn()
        {
            _sideMoveAnimator.AnimateOut();
            SetBtnsInteractable(false);
            ServiceLocator.Get<IUIManager>().Show<RelicsUIPanel>(UIConstants.UIRelics, () =>
            {
                _sideMoveAnimator.AnimateIn();
                SetBtnsInteractable(true);
            }).Show();
        }

        private void AltarsBtn()
        {
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ScreenDarkening.Animate(() =>
            {
                ServiceLocator.Get<IUIManager>().Show<AltarsOverviewUI>(UIConstants.UIAltars, () =>
                {
                    ServiceLocator.Get<TabsSwitcher>().SetGateTab();
                }).Show();
            }, null);
        
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
            ScreenDarkening.Animate(() =>
            {
                ServiceLocator.Get<IUIManager>().Show<SummoningUI>(UIConstants.UISummon, () => 
                { 
                    ServiceLocator.Get<TabsSwitcher>().SetGateTab();
                }).Show();
            }, null);
       
        }

        private void OpenGameModeMenu()
        {
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ScreenDarkening.Animate(() =>
            {
                ServiceLocator.Get<IUIManager>().Show<GameModeSelectionUI>(UIConstants.UIGameModeSelection, () => 
                { 
                    ServiceLocator.Get<TabsSwitcher>().SetGateTab();
                }).Show();
            }, null);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void SetBtnsInteractable(bool interactable)
        {
            _summonBtn.SetInteractable(interactable);
            _battleBtn.SetInteractable(interactable);
            _settingBtn.SetInteractable(interactable);
            _relicsBtn.SetInteractable(interactable);
        }
    }
}