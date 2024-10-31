using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Relics;
using RobotCastle.Shop;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class TabsSwitcher : MonoBehaviour
    {
        public MenuTabType TabType => _tabType;
        
        [SerializeField] private GameObject _worldBarracks;
        [SerializeField] private GameObject _worldGate;
        [SerializeField] private GameObject _worldHeroView;        
        [SerializeField] private Transform _camPointGate;
        [SerializeField] private Transform _camPointBarracks;
        [SerializeField] private Transform _camPointHeroView;
        private MenuTabType _tabType;

        public void SetNoneTab()
        {
            var ui = ServiceLocator.Get<IUIManager>();
            ui.Show<GateTabUI>(UIConstants.UIGateTab, () => {}).Close();
            ui.Show<PlayerDataPanelUI>(UIConstants.UIPlayerData, () => {}).Close();
            ui.Show<MainMenuTabsUI>(UIConstants.UIMainMenuTabs, () => {}).Close();
            CloseCurrent();
            _tabType = MenuTabType.None;

        }
        
        public void CloseCurrent()
        {
            switch (_tabType)
            {
                case MenuTabType.Barracks:
                    CloseBarracks();
                    break;
                case MenuTabType.Gate:
                    CloseGate();
                    break;
                case MenuTabType.Shop:
                    CloseShop();
                    break;
                case MenuTabType.HeroView:
                    CloseHeroView();
                    break;
            }
            _tabType = MenuTabType.None;
        }
        
        public void SetGateTab()
        {
            CloseCurrent();
            ShowGate();
            _tabType = MenuTabType.Gate;
        }

        public void SetBarracksTab()
        {
            CloseCurrent();
            ShowBarracks();
            _tabType = MenuTabType.Barracks;
        }

        public void SetShopsTab()
        {
            CloseCurrent();
            ShowShop();
            _tabType = MenuTabType.Shop;
        }

        public void SetHeroView()
        {
            CloseCurrent();
            ShowHeroView();
            _tabType = MenuTabType.HeroView;
        }

        private void ShowBarracks()
        {
            _worldBarracks.SetActive(true);
            var ui = ServiceLocator.Get<IUIManager>();
            ui.Show<PlayerDataPanelUI>(UIConstants.UIPlayerData, () => {}).Show();
            ui.Show<MainMenuTabsUI>(UIConstants.UIMainMenuTabs, () => {}).Show();
            ui.Show<GateTabUI>(UIConstants.UIGateTab, () => {}).Close();
            ServiceLocator.Get<MainMenuCamera>().SetPoint(_camPointBarracks);
            ServiceLocator.Get<TabsHighlighterUI>().HighlightTab(MenuTabType.Barracks);
            var bm = ServiceLocator.Get<BarracksManager>();
            bm.Show();
        }
        
        private void ShowGate()
        {
            _worldGate.SetActive(true);
            var ui = ServiceLocator.Get<IUIManager>();
            ui.Show<GateTabUI>(UIConstants.UIGateTab, () => {}).Show();
            ui.Show<PlayerDataPanelUI>(UIConstants.UIPlayerData, () => {}).Show();
            ui.Show<MainMenuTabsUI>(UIConstants.UIMainMenuTabs, () => {}).Show();
            ServiceLocator.Get<PlayersPartyGateDisplay>().SetFromSave();
            ServiceLocator.Get<MainMenuCamera>().SetPoint(_camPointGate);
            ServiceLocator.Get<TabsHighlighterUI>().HighlightTab(MenuTabType.Gate);
            var screen = ui.GetIfShown<RelicsUIPanel>(UIConstants.UIRelics);
            if (screen)
                screen.On();
        }
        
        private void ShowShop()
        {
            ServiceLocator.Get<TabsHighlighterUI>().HighlightTab(MenuTabType.Shop);
            var ui = ServiceLocator.Get<IUIManager>();
            var shop = ui.Show<ShopUI>(UIConstants.UIShop, () => {});
            shop.Show();
        }

        private void ShowHeroView()
        {
            _worldHeroView.SetActive(true);
            var ui = ServiceLocator.Get<IUIManager>();
            ui.Show<GateTabUI>(UIConstants.UIGateTab, () => {}).Close();
            ui.Show<PlayerDataPanelUI>(UIConstants.UIPlayerData, () => {}).Close();
            ui.Show<MainMenuTabsUI>(UIConstants.UIMainMenuTabs, () => {}).Close();
            ServiceLocator.Get<MainMenuCamera>().SetPoint(_camPointHeroView);
        }
        
        private void CloseBarracks()
        {
            _worldBarracks.SetActive(false);
            var bm = ServiceLocator.Get<BarracksManager>();
            bm.Hide();
        }

        private void CloseGate()
        {
            var screen = ServiceLocator.Get<IUIManager>().GetIfShown<RelicsUIPanel>(UIConstants.UIRelics);
            if (screen)
                screen.Off();
            _worldGate.SetActive(false);
        }
        
        private void CloseShop()
        {
            var ui = ServiceLocator.Get<IUIManager>();
            var shop = ui.Show<ShopUI>(UIConstants.UIShop, () => {});
            shop.Hide();
        }

        private void CloseHeroView()
        {
            _worldHeroView.SetActive(false);
            ServiceLocator.Get<BarracksHeroView>().Close();
        }
    }
}