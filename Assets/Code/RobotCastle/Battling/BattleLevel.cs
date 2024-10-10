using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using RobotCastle.Battling.DevilsOffer;
using RobotCastle.Battling.MerchantOffer;
using RobotCastle.Battling.SmeltingOffer;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.MainMenu;
using RobotCastle.Merging;
using RobotCastle.Saving;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(35)]
    public partial class BattleLevel : MonoBehaviour, IBattleStartedProcessor, IBattleEndProcessor
    {
        [SerializeField] private bool _logRewardItems;
        [SerializeField] private int _startMoney = 9;
        [SerializeField] private bool _fillActiveAreaBeforeStart = true;
        [SerializeField] private float _endDelay = 2f;
        [SerializeField] private float _startEnemyShowTime = .5f;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private MapBuilder _map;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private BattleCamera _battleCamera;
        [SerializeField] private BattleGridSwitch _gridSwitch;
        [SerializeField] private CastleHealthView _playerHealthView;
        [SerializeField] private ParticleSystem _troopSizeExpansion;
        [Space(10)]
        [SerializeField] private SmeltingConfigContainer _smeltingConfigContainer;
        [SerializeField] private DevilsOfferConfigContainer _devilsOfferConfigContainer;
        [SerializeField] private MerchantOfferConfigContainer _merchantOfferConfigContainer;
        private IPlayerMergeItemPurchaser _itemPurchaser;
        private ITroopSizeManager _troopSizeManager;
        private BattleMergeUI _mainUI;
        private Chapter _chapter;
        private ChapterSelectionData _selectionData;
        private CancellationTokenSource _token;
        private SmeltingOfferManager _smeltingOffer;
        private DevilsOfferManager _devilsOffer;
        private MerchantOfferManager _merchantOffer;

        public void OnBattleStarted(Battle battle)
        {
            AllowPlayerUIInput(true);
            _gridSwitch.SetBattleMode();
            _mergeManager.AllowInput(false);
            ServiceLocator.Get<MergeManager>().StopHighlight();
        }

        public void OnBattleEnded(Battle battle)
        {
            BattleRoundCompletion(battle, _token.Token);
        }

        private void Awake()
        {
            GameState.Mode = GameState.EGameMode.InvasionBattle;
            if(!SleepDev.AdsPlayer.Instance.BannerCalled)
                SleepDev.AdsPlayer.Instance.ShowBanner();
        }

        private void Start()
        {
            var playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            _selectionData = playerData.chapterSelectionData;
            SleepDev.Analytics.OnLevelStarted(_selectionData.chapterIndex, _selectionData.tierIndex);
            _chapter = ServiceLocator.Get<ProgressionDataBase>().chapters[_selectionData.chapterIndex];
            _token = new CancellationTokenSource();
            ServiceLocator.Get<GameMoney>().levelMoney = _startMoney;
            try
            {
                CLog.Log($"Start init");
                Init(_token.Token);
            }catch(System.Exception ex)
            {
                CLog.Log(ex.Message);    
            }
        }

        private async Task Init(CancellationToken token)
        {
            CLog.Log($"Start INIT CALL");
            var uiMan = ServiceLocator.Get<IUIManager>(); 
            _map.InitRuntime();
            ServiceLocator.Bind<IMap>(_map.Map);
            ServiceLocator.Bind<BattleCamera>(_battleCamera);
            ServiceLocator.Bind<EnemiesManager>(_enemiesManager);
            ServiceLocator.Bind<CastleHealthView>(_playerHealthView);
            uiMan.ParentCanvas = _mainCanvas;
            uiMan.Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            
            _battleManager.startProcessor = this;
            _battleManager.endProcessor = this;
            var battle = _battleManager.Init(_chapter.levelData.levels);
            _troopSizeManager = new BattleTroopSizeManager(battle, _troopSizeExpansion);
            ServiceLocator.Bind<ITroopSizeManager>(_troopSizeManager);
            _mergeManager.Init();
            _enemiesManager.Init();
            _playerHealthView.SetHealth(battle.playerHealthPoints);
            _itemPurchaser = gameObject.GetComponent<IPlayerMergeItemPurchaser>();
            _smeltingOffer = new SmeltingOfferManager(_smeltingConfigContainer.config, _mergeManager.GridView, _mergeManager.SectionsController, _troopSizeManager, _battleManager.battle);
            _devilsOffer = new DevilsOfferManager(_devilsOfferConfigContainer.config, _mergeManager.GridView, _mergeManager.SectionsController, _troopSizeManager, _battleManager, _playerHealthView);
            _merchantOffer = new MerchantOfferManager(_merchantOfferConfigContainer.config, _mergeManager.GridView, _mergeManager.SectionsController, _troopSizeManager, _battleManager, _playerHealthView);
            try
            {
                CLog.Log($"Awaiting settings stage");
                await _battleManager.SetStage(0, token);
            }
            catch (System.Exception ex)
            {
                CLog.LogError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
            _battleCamera.AllowPlayerInput(false);
            _battleCamera.SetBattlePoint();
            await Task.Delay((int)(_startEnemyShowTime * 1000), token);
            InitUI();
            await _battleCamera.MoveToMergePoint();
            GrantPlayerInput();
        }
        
        private void InitUI()
        {
            var uiManager = ServiceLocator.Get<IUIManager>();
            uiManager.Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
            var damageUI = uiManager.Show<BattleDamagePanelUI>("ui_damage", () => { });
            damageUI.gameObject.SetActive(true);
            
            _mainUI = uiManager.Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            _mainUI.BtnSpawn.AddMainCallback(BuyNewItem);
            _mainUI.BtnStart.AddMainCallback(StartBattle);
            _mainUI.BtnStart2.AddMainCallback(StartBattle);
            _mainUI.TroopSizePurchaseUI.Init(_troopSizeManager);
            _mainUI.TroopSizePurchaseUI.SetInteractable(true);
            _mainUI.Init(_battleManager.battle, _chapter);
            InitCurrentRound();
        }

        private void StartBattle()
        {
            if (_battleManager.battle.State == BattleState.Going)
            {
                CLog.LogError($"Battle is already going!");
                return;
            }
            if (_battleManager.CanStart())
            {
                StartingBattle(_token.Token);
            }
        }

        private async Task StartingBattle(CancellationToken token)
        {
            AllowPlayerUIInput(false);
            if(_fillActiveAreaBeforeStart)
                _mergeManager.FillActiveArea();
            _battleManager.SetGoingState();
            _mainUI.Started();
            foreach (var hero in _battleManager.battle.playersAlive)
                hero.View.transform.rotation = Quaternion.Euler(Vector3.zero);
            await _battleCamera.MoveToBattlePoint();
            if (token.IsCancellationRequested) return;
            _battleManager.BeginBattle();
        }

        private void BuyNewItem()
        {
            _itemPurchaser.TryPurchaseItem();
        }

        private async Task BattleRoundCompletion(Battle battle, CancellationToken token)
        {
            CLog.Log($"[{nameof(BattleLevel)}] Round index: {battle.roundIndex}. Max Index: {_chapter.levelData.levels.Count - 1}");
            if (battle.roundIndex >= _chapter.levelData.levels.Count - 1)
            {
                CLog.Log($"Level Completed!");
                SleepDev.Analytics.OnLevelCompleted(_selectionData.chapterIndex, _selectionData.tierIndex);
                GiveRewardAndShowUI();
                return;
            }
            _battleManager.BattleRewardCalculator.AddRewardForStage();
            await Task.Delay((int)(_endDelay * .5f * 1000), token);
            _gridSwitch.SetMergeMode();
            switch (battle.State)
            {
                case BattleState.EnemyWin:
                    if(CheckLevelLost())
                        return;
                    await _battleManager.ResetStage(token);
                    break;
                case BattleState.PlayerWin:
                    _battleManager.SetNextStage();
                    _mainUI.UpdateForNextWave();
                    _mainUI.Win();
                    await _battleManager.SetCurrentStage(token);
                    break;
                default:
                    CLog.LogRed($"[{nameof(BattleLevel)}] Error!");
                    break;
            }
            await Task.Yield();
            if (token.IsCancellationRequested) return;

            _battleManager.RecollectPlayerUnits();

            await Task.Delay((int)(_endDelay * .5f * 1000), token);
            if (token.IsCancellationRequested) return;
            
            var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
            if(battleUI)
                battleUI.SetMainAreaUpPos();
            
            await _battleCamera.MoveToMergePoint();
            if (token.IsCancellationRequested) return;
            
            InitCurrentRound();
        }
        
        
        private void InitCurrentRound()
        {
            var roundType = _chapter.levelData.levels[_battleManager.battle.roundIndex].roundType;
            switch (roundType)
            {
                case RoundType.Default:
                    GrantPlayerInput();
                    break;
                case RoundType.Smelting:
                    ShowSmeltingOffer();
                    break;
                case RoundType.EliteEnemy:
                    ShowDevilsOfferAndTrade();
                    break;
                case RoundType.Boss:
                    CLog.Log($"[{nameof(BattleLevel)}] Boss level");
                    GrantPlayerInput();
                    break;
            }
        }

        private void ShowSmeltingOffer()
        {
            _smeltingOffer.MakeNextOffer(GrantPlayerInput);
        }

        private void ShowDevilsOffer()
        {
            _devilsOffer.MakeNextOffer(GrantPlayerInput);
        }

        private void ShowDevilsOfferAndTrade()
        {
            _devilsOffer.MakeNextOffer(ShowTradeOffer);
        }

        private void ShowTradeOffer()
        {
            _merchantOffer.MakeNextOffer(GrantPlayerInput);
        }
        
        private void GrantPlayerInput()
        {
            _mergeManager.AllowInput(true);
            AllowPlayerUIInput(true);
            _battleCamera.AllowPlayerInput(true);
        }

        private void AllowPlayerUIInput(bool allow)
        {
            _mainUI.BtnSpawn.SetInteractable(allow);
            _mainUI.BtnStart.SetInteractable(allow);
            _mainUI.BtnStart2.SetInteractable(allow);
            _mainUI.TroopSizePurchaseUI.SetInteractable(allow);
        }
        
        private void ShowFailedScreen()
        {
            _mergeManager.AllowInput(false);
            var lvlFail = ServiceLocator.Get<IUIManager>().Show<InvasionLevelFailUI>(UIConstants.UILevelFail, () =>{});
            lvlFail.Show(Replay, ReturnToMenu);
        }
        
        private bool CheckLevelLost()
        {
            var health = --_battleManager.battle.playerHealthPoints;
            if (health <= 0)
                health = 0;
            _playerHealthView.MinusHealth(health);
            _mainUI.Lost();
            if (health == 0)
            {
                CLog.LogWhite($"[{nameof(BattleLevel)}] Player LOST");
                AllowPlayerUIInput(false);
                ShowFailedScreen();
                return true;
            }
            return false;
        }

        private void SetTierCompletedAndUnlockNext()
        {
            var playerData = DataHelpers.GetPlayerData();
            var tier = _selectionData.tierIndex;
            var chapterSave = playerData.progression.chapters[_selectionData.chapterIndex];
            var tierSave = chapterSave.tierData[tier];
            tierSave.completed = true;
            tierSave.completedCount++;
            var chapterConfig = ServiceLocator.Get<ProgressionDataBase>().chapters[_selectionData.chapterIndex];
            tier++;
            if (tier < chapterSave.tierData.Count)
            {
                chapterSave.tierData[tier].unlocked = true;
            }
            else if(_selectionData.chapterIndex < playerData.progression.chapters.Count)
            {
                var nextChapterSave = playerData.progression.chapters[_selectionData.chapterIndex + 1];
                nextChapterSave.unlocked = true;
                nextChapterSave.tierData[0].unlocked = true;
            }
        }
        
        
        private void GiveRewardAndShowUI()
        {
            var chapterConfig = ServiceLocator.Get<ProgressionDataBase>().chapters[_selectionData.chapterIndex];
            var playerData = DataHelpers.GetPlayerData();
            SetTierCompletedAndUnlockNext();
            var rewardMultiplier = chapterConfig.tiers[_selectionData.tierIndex].multiplier;
            var goldReward = Mathf.RoundToInt(rewardMultiplier * chapterConfig.moneyGoldReward);
            var xpReward = Mathf.RoundToInt(rewardMultiplier * chapterConfig.playerXpReward);

            var addedHeroesXp = Mathf.RoundToInt(rewardMultiplier * chapterConfig.heroXpReward);
            var heroesSaves = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            foreach (var id in playerData.party.heroesIds)
                heroesSaves.GetSave(id).xp += addedHeroesXp;

            var gm = ServiceLocator.Get<GameMoney>();
            gm.globalMoney += goldReward;
            var newPlayerXp = ServiceLocator.Get<CastleXpManager>().AddXp(xpReward);
            
            var rewardsList = new List<CoreItemData>(5);
            rewardsList.Add(new CoreItemData(goldReward, "gold", "bonus"));
            rewardsList.Add(new CoreItemData(goldReward, "player_xp", "bonus"));
            var inventory = playerData.inventory;
            foreach (var reward in chapterConfig.chapterCompletedRewards)
            {
                var upgReward = new CoreItemData(reward);
                upgReward.level = Mathf.RoundToInt(upgReward.level * rewardMultiplier);
                rewardsList.Add(upgReward);
                if(reward.type == "item")
                    inventory.AddItem(reward.id, reward.level);
                else
                    CLog.LogRed($"reward type != \"item\", will not add to inventory!");
            }

            if (_logRewardItems)
            {
                var msg = "Added rewards after level win!";
                foreach (var it in rewardsList)
                    msg += $"\n{it.id}  {it.level}";
                CLog.Log(msg);          
            }
            
            var addedXps = new List<float>(6){addedHeroesXp,addedHeroesXp,addedHeroesXp,addedHeroesXp,addedHeroesXp,addedHeroesXp};
            var ui = ServiceLocator.Get<IUIManager>().Show<InvasionLevelWinUI>(UIConstants.UILevelWin, () => {});
            ui.Show(new InvasionWinArgs()
            {
                playerNewLevelReached = newPlayerXp,
                playerXpAdded = xpReward,
                heroesXp = addedXps,
                selectionData = _selectionData,
                rewards = rewardsList,
                replayCallback = Replay,
                returnCallback = ReturnToMenu
            });
        }

        private void ReturnToMenu()
        {
            CLog.Log($"[{nameof(BattleLevel)}] Return to menu call");
            SceneManager.LoadScene(GlobalConfig.SceneMainMenu);
        }

        private void Replay()
        {
            CLog.Log($"[{nameof(BattleLevel)}] Replay call");
            SleepDev.Analytics.LevelReplayCalled(_selectionData.chapterIndex, _selectionData.tierIndex);
            var playerData = DataHelpers.GetPlayerData();
            playerData.playerEnergy -= ChapterSelectionData.BasicEnergyCost;
            SceneManager.LoadScene(GlobalConfig.SceneBattle);
        }
        
    }
}