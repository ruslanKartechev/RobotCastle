using System.Collections;
using System.Net;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using SleepDev.FlyingUI;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    [DefaultExecutionOrder(1)]
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
        [SerializeField] private FlyingUIScreen _flyingUIScreen;

        private void Awake()
        {
            ServiceLocator.Get<IUIManager>().Refresh();
            GameState.Mode = GameState.EGameMode.MainMenu;
        }

        private void Start()
        {
            ServiceLocator.Bind<MainMenuCamera>(_camera);
            ServiceLocator.Bind<BarracksHeroesPool>(_heroesPool);
            ServiceLocator.Bind<TabsSwitcher>(_tabsSwitcher);
            ServiceLocator.Bind<PlayersPartyGateDisplay>(_partyGateDisplay);
            ServiceLocator.Bind<IMergeItemsFactory>(_mergeItems);
            ServiceLocator.Bind<BarracksManager>(_barracksManager);
            ServiceLocator.Bind<BarracksHeroView>(_barrackHeroView);
            ServiceLocator.Bind<FlyingUIScreen>(_flyingUIScreen);

            var ui = ServiceLocator.Get<IUIManager>();
            ui.ParentCanvas = _parentCanvas;
            ui.AddAsShown(UIConstants.UIMainMenuTabs, _tabs);
            ui.AddAsShown(UIConstants.UIGateTab, _gateTabUI);
            ui.AddAsShown(UIConstants.UIPlayerData, _playerPanel);
            _barracksManager.Init();

            _heroesPool.SpawnAll();
            _tabsSwitcher.SetGateTab();

            AdsPlayer.Instance.ShowBanner();

            DataHelpers.SaveData();
            TryStartTutorial();
        }

        private void TryStartTutorial()
        {
            var playerData = DataHelpers.GetPlayerData();
            var save = playerData.tutorials;
            if (save.enterPlay == false)
            {
                var canvas = ServiceLocator.Get<IUIManager>().ParentCanvas;
                var prefab = Resources.Load<TutorialBase>("prefabs/tutorials/ui_tutor_enter_play");
                var instance = Instantiate(prefab, canvas.transform);
                instance.Begin(() =>
                {
                    save.enterPlay = true;
                    CLog.Log($"Enter Play tutor completed");
                });
            }
            else if (save.enterPlay && !save.heroUpgrade)
            {
                var lvl2Unlocked = playerData.progression.chapters[0].tierData[1].unlocked;
                if (!lvl2Unlocked)
                    return;
                var canvas = ServiceLocator.Get<IUIManager>().ParentCanvas;
                var prefab = Resources.Load<TutorialBase>("prefabs/tutorials/ui_tutor_barracks");
                var instance = Instantiate(prefab, canvas.transform);
                instance.Begin(() =>
                {
                    save.heroUpgrade = true;
                    CLog.Log($"Barracks tutorial completed");
                });
            }
            else if (save.heroUpgrade && !save.heroSummon)
            {
                var canvas = ServiceLocator.Get<IUIManager>().ParentCanvas;
                var prefab = Resources.Load<TutorialBase>("prefabs/tutorials/ui_tutor_summon");
                var instance = Instantiate(prefab, canvas.transform);
                instance.Begin(() =>
                {
                    save.heroSummon = true;
                    CLog.Log($"Summon tutorial completed");
                });
            }
            else if(save.heroSummon && !save.altars && playerData.playerLevel > 0)
            {
                var canvas = ServiceLocator.Get<IUIManager>().ParentCanvas;
                var prefab = Resources.Load<TutorialBase>("prefabs/tutorials/ui_tutor_altars");
                var instance = Instantiate(prefab, canvas.transform);
                instance.Begin(() =>
                {
                    save.altars = true;
                    CLog.Log($"Altars tutorial completed");
                });
            }
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
