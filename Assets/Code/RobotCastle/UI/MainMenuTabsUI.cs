using RobotCastle.Core;
using RobotCastle.MainMenu;
using UnityEngine;

namespace RobotCastle.UI
{
    public class MainMenuTabsUI : MonoBehaviour, IScreenUI
    {
        public MyButton gateBtn => _gateBtn;
   
        public MyButton barracksBtn => _barracksBtn;

        public MyButton shopBtn => _shopBtn;

        public Transform ButtonsParent => _buttonsParent;

        [SerializeField] private Transform _buttonsParent;
        [SerializeField] private MyButton _gateBtn;
        [SerializeField] private MyButton _barracksBtn;
        [SerializeField] private MyButton _shopBtn;
        
        private void Start()
        {
            _gateBtn.AddMainCallback(Gate);
            _barracksBtn.AddMainCallback(Battle);
            _shopBtn.AddMainCallback(Shop);
            SetAllInteractable(true);
        }
        
        public void SetAllInteractable(bool interactable)
        {
            _gateBtn.SetInteractable(interactable);
            _barracksBtn.SetInteractable(interactable);
            _shopBtn.SetInteractable(interactable);
        }

        private void Shop()
        {
            ServiceLocator.Get<TabsSwitcher>().SetShopsTab();
        }

        private void Gate()
        {
            ServiceLocator.Get<TabsSwitcher>().SetGateTab();
        }

        private void Battle()
        {
            ServiceLocator.Get<TabsSwitcher>().SetBarracksTab();
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