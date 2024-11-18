using RobotCastle.Battling.Altars;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.MainMenu;
using RobotCastle.Summoning;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BarracksTabUI : MonoBehaviour, IScreenUI
    {
        public MyButton altarsBtn => _altarsBTN;
        
        public MyButton summonBtn => _summonBtn;

        public void Show() => gameObject.SetActive(true);

        public void Close() => gameObject.SetActive(false);
        
        public void SetBtnsInteractable(bool interactable)
        {
            _summonBtn.SetInteractable(interactable);
            _altarsBTN.SetInteractable(interactable);
        }
        
        [SerializeField] private MyButton _altarsBTN;
        [SerializeField] private MyButton _summonBtn;
        
        private void Start()
        {
            _summonBtn.AddMainCallback(OpenSummonMenu);
            _altarsBTN.AddMainCallback(OnAltarsBtn);
            SetBtnsInteractable(true);
        }
        
        private void OnAltarsBtn()
        {
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ScreenDarkening.Animate(() =>
            {
                ServiceLocator.Get<IUIManager>().Show<AltarsOverviewUI>(UIConstants.UIAltars, () =>
                {
                    ServiceLocator.Get<TabsSwitcher>().SetBarracksTab();
                }).Show();
            }, null);        
        }
        
        private void OpenSummonMenu()
        {
            ServiceLocator.Get<TabsSwitcher>().SetNoneTab();
            ScreenDarkening.Animate(() =>
            {
                ServiceLocator.Get<IUIManager>().Show<SummoningUI>(UIConstants.UISummon, () => 
                { 
                    ServiceLocator.Get<TabsSwitcher>().SetBarracksTab();
                }).Show();
            }, null);
        }

    }
}