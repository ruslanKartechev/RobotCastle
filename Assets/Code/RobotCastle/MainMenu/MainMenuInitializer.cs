using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class MainMenuInitializer : MonoBehaviour
    {
        [SerializeField] private Canvas _parentCanvas;
        [SerializeField] private MainMenuCamera _camera;
        [SerializeField] private BarracksHeroesPool _heroesPool;
        [SerializeField] private TabsSwitcher _tabsSwitcher;
        [SerializeField] private PlayersPartyGateDisplay _partyGateDisplay;
        [SerializeField] private GateTabUI _gateTabUI;
        [SerializeField] private PlayerDataPanelUI _playerPanel;
        [SerializeField] private MainMenuTabsUI _tabs;
        [SerializeField] private MergeItemsFactory _mergeItems;
        [SerializeField] private BarracksManager _barracksManager;
        [SerializeField] private BarracksHeroView _barrackHeroView;
        
        private void Start()
        {
            GameState.Mode = GameState.EGameMode.MainMenu;
            ServiceLocator.Bind<MainMenuCamera>(_camera);
            ServiceLocator.Bind<BarracksHeroesPool>(_heroesPool);
            ServiceLocator.Bind<TabsSwitcher>(_tabsSwitcher);
            ServiceLocator.Bind<PlayersPartyGateDisplay>(_partyGateDisplay);
            ServiceLocator.Bind<IMergeItemsFactory>(_mergeItems);
            ServiceLocator.Bind<BarracksManager>(_barracksManager);
            ServiceLocator.Bind<BarracksHeroView>(_barrackHeroView);
            var ui = ServiceLocator.Get<IUIManager>();
            ui.ParentCanvas = _parentCanvas;
            ui.AddAsShown(UIConstants.UIMainMenuTabs, _tabs);
            ui.AddAsShown(UIConstants.UIGateTab, _gateTabUI);
            ui.AddAsShown(UIConstants.UIPlayerData, _playerPanel);
            _barracksManager.Init();
            
            _heroesPool.SpawnAll();
            _tabsSwitcher.SetGateTab();
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<MainMenuCamera>();
            ServiceLocator.Unbind<BarracksHeroesPool>();
            ServiceLocator.Unbind<TabsSwitcher>();
            ServiceLocator.Unbind<PlayersPartyGateDisplay>();
            ServiceLocator.Unbind<BarracksManager>();
            ServiceLocator.Unbind<BarracksHeroView>();
        }
    }
}
