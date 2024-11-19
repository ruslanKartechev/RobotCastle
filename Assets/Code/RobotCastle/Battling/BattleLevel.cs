using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using RobotCastle.Battling.Altars;
using RobotCastle.Battling.DevilsOffer;
using RobotCastle.Battling.MerchantOffer;
using RobotCastle.Battling.SmeltingOffer;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.MainMenu;
using RobotCastle.Merging;
using RobotCastle.Relics;
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
        [SerializeField] private int _startRoundIndex = 0;
        [SerializeField] private bool _logRewardItems;
        [SerializeField] private bool _fillActiveAreaBeforeStart = true;
        [SerializeField] private float _endDelay = 2f;
        [SerializeField] private float _startEnemyShowTime = .5f;
        [SerializeField] private float _winFailDelay = .5f;
        [SerializeField] private float _delayBetweenOffers = .6f;
        [Space(10)]
        [SerializeField] private BattleStartLevelData _startData;
        [Space(10)]
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private MapBuilder _map;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private BattleCamera _battleCamera;
        [SerializeField] private BattleGridSwitch _gridSwitch;
        [SerializeField] private CastleHealthView _playerHealthView;
        [SerializeField] private ParticleSystem _troopSizeExpansion;
        [SerializeField] private BattleLocationSpawner _locationSpawner;
        [Space(10)]
        [SerializeField] private SmeltingConfigContainer _smeltingConfigContainer;
        [SerializeField] private DevilsOfferConfigContainer _devilsOfferConfigContainer;
        [SerializeField] private MerchantOfferConfigContainer _merchantOfferConfigContainer;
        [Space(10)]
        [SerializeField] private ObstaclesContainer _obstacles;
        [SerializeField] private ObstaclesManager _obstaclesManager;
        [Space(10)] 
        [SerializeField] private SoundID _battleStartSound;
        
        private IPlayerFactory _playerMergeFactory;
        private ITroopSizeManager _troopSizeManager;
        private BattleMergeUI _mainUI;
        private Chapter _chapter;
        private ChapterSelectionData _selectionData;
        private CancellationTokenSource _token;
        private SmeltingOfferManager _smeltingOffer;
        private DevilsOfferManager _devilsOffer;
        private MerchantOfferManager _merchantOffer;
        private SavePlayerData _playerData;
        private IUIManager _uiManager;
        private int _roundAttemptsCount = 0;

        
        public void OnBattleStarted(Battle battle)
        {
            // AllowPlayerUIInput(true);
            _gridSwitch.SetBattleMode();
            _mergeManager.AllowInput(false);
            ServiceLocator.Get<MergeManager>().StopHighlight();
        }

        public void OnBattleEnded(Battle battle)
        {
            BattleRoundCompletion(battle, _token.Token);
        }


        private void Start()
        {
            ServiceLocator.Get<IUIManager>().Refresh();
            GameState.Mode = GameState.EGameMode.InvasionBattle;
            _playerData = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
            _selectionData = _playerData.chapterSelectionData;
            _uiManager = ServiceLocator.Get<IUIManager>(); 
            _chapter = ServiceLocator.Get<ProgressionDataBase>().chapters[_selectionData.chapterIndex];
            _playerMergeFactory = gameObject.GetComponent<IPlayerFactory>();
            SleepDev.Analytics.OnLevelStarted(_selectionData.chapterIndex, _selectionData.tierIndex);
            _token = new CancellationTokenSource();
            MoveCameraAndInit(_token.Token);
        }
        

        private void SetLocation()
        {
            _locationSpawner.SpawnLocation(_chapter);
            SetObstacles();
        }

        private void SetObstacles()
        {
            _obstaclesManager.SetMap(_map.Map);
            var preset = _obstacles.options.Random();
            _obstaclesManager.SetPreset(preset, _obstacles.prefabsOptions[_selectionData.chapterIndex]);
        }

        private void AddTierBasedDifficultyModifiers()
        {
            var tiersDb = ServiceLocator.Get<DifficultyTiersDatabase>();
            var config = tiersDb.tiersConfig[_selectionData.tierIndex];
            var roundsCount = _chapter.levelData.levels.Count;
            if (config.enemyForcesPercent > 0)
            {
                _battleManager.AddRoundModifier(new RoundModifierEnemyForcesIncrease(config.enemyForcesPercent, roundsCount));
            }
            if (config.enemyTier > 0)
            {
                _battleManager.AddRoundModifier(new RoundModifierEnemiesTierUp(config.enemyTier, roundsCount));
            }
            if (config.additionalEliteEnemies > 0)
            {
                _battleManager.AddRoundModifier(new RoundModifierEliteEnemies(config.additionalEliteEnemies));
            }
            if (config.castleDurabilityPenalty > 0)
            {
                var health = _battleManager.battle.playerHealthPoints - config.castleDurabilityPenalty;
                _battleManager.battle.playerHealthPoints = health;
                _playerHealthView.MinusHealth(health);
            }
        }

        private void BindRefs(bool bind)
        {
            if (bind)
            {
                ServiceLocator.Bind<IMap>(_map.Map);
                ServiceLocator.Bind<BattleCamera>(_battleCamera);
                ServiceLocator.Bind<EnemiesManager>(_enemiesManager);
                ServiceLocator.Bind<CastleHealthView>(_playerHealthView);
                ServiceLocator.Bind<ITroopSizeManager>(_troopSizeManager);
                ServiceLocator.Bind<IPlayerFactory>(_playerMergeFactory);
                ServiceLocator.Bind<IBattleStartData>(_startData);
                ServiceLocator.Bind<BattleLevel>(this);
                
            }
            else
            {
                ServiceLocator.Unbind<IMap>();
                ServiceLocator.Unbind<BattleCamera>();
                ServiceLocator.Unbind<EnemiesManager>();
                ServiceLocator.Unbind<CastleHealthView>();
                ServiceLocator.Unbind<ITroopSizeManager>();
                ServiceLocator.Unbind<IPlayerFactory>();
                ServiceLocator.Unbind<IBattleStartData>();
                ServiceLocator.Unbind<BattleLevel>();
            }
        }

        private void OnDisable()
        {
            _token?.Cancel();
            BindRefs(false);
        }

        private void InitOffers()
        {
            CLog.Log($"[{nameof(BattleLevel)}] Init Offers");
            _smeltingOffer = new SmeltingOfferManager(_smeltingConfigContainer.config);
            _devilsOffer = new DevilsOfferManager(_devilsOfferConfigContainer.config, _battleManager, _playerHealthView);
            _merchantOffer = new MerchantOfferManager(_merchantOfferConfigContainer.config, _battleManager);
            ServiceLocator.Bind<SmeltingOfferManager>(_smeltingOffer);
            ServiceLocator.Bind<DevilsOfferManager>(_devilsOffer);
            ServiceLocator.Bind<MerchantOfferManager>(_merchantOffer);
        }

        private void InitAltars()
        {
            CLog.Log($"[{nameof(BattleLevel)}] Init Altars");
            var manager = ServiceLocator.Get<AltarManager>();
            manager.SetupAndApplyAll();
        }

        private void InitRelics()
        {
            CLog.Log($"[{nameof(BattleLevel)}] Applying relics");
            RelicsManager.ApplyAllSelectedRelics();
        }
        
        private void SetStartData()
        {
            CLog.Log($"[{nameof(BattleLevel)}] Start money: {_startData.StartMoney}. Start items: {_startData.StartItems.Count}");
            ServiceLocator.Get<GameMoney>().levelMoney.SetValue(_startData.StartMoney);
            foreach (var item in _startData.StartItems)
            {
                _mergeManager.SpawnNewMergeItem(new SpawnArgs(item));
            }
        }
        
        private void InitUI()
        {
            var uiManager = ServiceLocator.Get<IUIManager>();
            uiManager.Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
            var damageUI = uiManager.Show<BattleDamagePanelUI>("ui_damage", () => { });
            damageUI.gameObject.SetActive(true);
            
            _mainUI = uiManager.Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            _mainUI.BtnStart.AddMainCallback(StartBattle);
            _mainUI.BtnStart2.AddMainCallback(StartBattle);
            _mainUI.TroopSizePurchaseUI.SetInteractable(true);
            _mainUI.Init(_battleManager.battle, _chapter);
            _mainUI.StatsCollector.SetCollector((BattleDamageStatsCollector)_battleManager.playerStatCollector);
            _mainUI.AnimateShowUp();
            InitCurrentRound(_roundAttemptsCount);
        }
        
        private async void MoveCameraAndInit(CancellationToken token)
        {
            await Task.WhenAll(InitProcess(token), 
                _battleCamera.PlayStartAnimation(token));
            if(token.IsCancellationRequested)return;
            var save = DataHelpers.GetPlayerData().tutorials;
            if (!save.battle)
            {
                var canvas = ServiceLocator.Get<IUIManager>().ParentCanvas;
                var prefab = Resources.Load<TutorialBattle>("prefabs/tutorials/ui_tutor_battle");
                var instance = Instantiate(prefab, canvas.transform);
                instance.Begin(() => {
                    save.battle = true;
                });
            }
        }

        private Battle CreateBattle()
        {
            var lvlData = new LevelData(_chapter.levelData);
            if (_selectionData.tierIndex > 0)
            {
                lvlData.levels[^1].enemyPreset += "_boss";
            }
            return _battleManager.CreateBattle(lvlData);
        }
        
        private async Task InitProcess(CancellationToken token)
        {
            CLog.Log($"[{nameof(BattleLevel)}] Init process");
            _battleCamera.SetBattlePoint();
            _battleCamera.AllowPlayerInput(false);
            _uiManager.ParentCanvas = _mainCanvas;
            _uiManager.Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
            _map.InitRuntime();
            SetLocation();
            await Task.Yield();
            if (token.IsCancellationRequested) return;

            _battleManager.startProcessor = this;
            _battleManager.endProcessor = this;

            var battle = CreateBattle();
            _mergeManager.Init();
            _troopSizeManager = new BattleTroopSizeManager(battle, _mergeManager.SectionsController, _troopSizeExpansion);
            _enemiesManager.Init();
            _playerHealthView.SetHealth(battle.playerHealthPoints);
            
            BindRefs(true);
            try
            {
                AddTierBasedDifficultyModifiers();
            } catch(System.Exception ex) { CLog.LogException(ex.Message, ex.StackTrace); }

            try
            { 
                InitOffers();
            } catch(System.Exception ex) { CLog.LogException(ex.Message, ex.StackTrace); }

            try
            {
                InitAltars();
            } catch(System.Exception ex) { CLog.LogException(ex.Message, ex.StackTrace); }

            try
            {
                InitRelics();
            } catch(System.Exception ex) { CLog.LogException(ex.Message,  ex.StackTrace); }

            try
            {
                SetStartData();
            } catch(System.Exception ex) { CLog.LogException(ex.Message, ex.StackTrace); }
            
            
            await Task.Yield();
            if (token.IsCancellationRequested) return;

            try
            {
                if (_startRoundIndex >= _chapter.levelData.levels.Count)
                    _startRoundIndex = 0;
                await _battleManager.SetRound(_startRoundIndex, token);
            }
            catch (System.Exception ex) { CLog.LogError($"Exception: {ex.Message}\n{ex.StackTrace}"); }
            
            await Task.Delay((int)(_startEnemyShowTime * 1000), token);
            if (token.IsCancellationRequested) return;
            
            try
            {
                InitUI();
            } catch(System.Exception ex) { CLog.LogError($"{ex.Message}\n{ex.StackTrace}"); }
            
            GrantPlayerInput();
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
                try
                {
                    StartingBattle(_token.Token);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"{ex.Message}. {ex.StackTrace}");                    
                }
            }
            else
            {
                var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {});
                ui.ShowNoHeroesOnGrid();
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
                hero.Components.transform.rotation = Quaternion.Euler(Vector3.zero);
            
            SoundManager.Inst.Play(_battleStartSound);
            _battleCamera.Stop();
            await _battleCamera.MovingAndScalingToBattle(token);
            
            if (token.IsCancellationRequested) return;
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            await Task.Delay(110, token);
            try
            {
                _battleManager.BeginBattle();
            }
            catch (System.Exception ex)
            {
                CLog.LogError(ex.Message, ex.StackTrace);
            }
        }

        private async Task BattleRoundCompletion(Battle battle, CancellationToken token)
        {
            var didWin = battle.State == BattleState.PlayerWin;
            CLog.Log($"[{nameof(BattleLevel)}] Round index: {battle.roundIndex}, status: {didWin}");
            if (didWin && battle.roundIndex >= _chapter.levelData.levels.Count - 1)
            {
                CLog.Log($"[Battle Level] Level Completed!");
                await Task.Delay((int)(_winFailDelay * 1000), token);
                if (token.IsCancellationRequested) return;
                SleepDev.Analytics.OnLevelCompleted(_selectionData.chapterIndex, _selectionData.tierIndex);
                AddRewardForLevelAndShowUI();
                return;
            }
            _battleManager.rewardCalculator.AddRewardForStage(didWin);
            await Task.Delay((int)(_endDelay * .5f * 1000), token);
            if (token.IsCancellationRequested) return;

            _gridSwitch.SetMergeMode();
            if (didWin)
            {
                _roundAttemptsCount = 0;
                _battleManager.SetNextStage();
                _mainUI.UpdateForNextWave();
                _mainUI.Win();
                await _battleManager.SetCurrentStage(token);
            }
            else
            {
                _roundAttemptsCount++;
                if(SubtractHealthAndCheckLoose())
                    return;
                await _battleManager.ResetStage(token);
            }
        
            await Task.Yield();
            if (token.IsCancellationRequested) return;

            _battleManager.RecollectPlayerUnits();

            await Task.Delay((int)(_endDelay * .5f * 1000), token);
            if (token.IsCancellationRequested) return;
            
            var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
            if(battleUI)
                battleUI.SetMainAreaUpPos();
            
            _battleCamera.Stop();
            await _battleCamera.MovingAndScalingToMerge(token);
            
            if (token.IsCancellationRequested) return;
            
            InitCurrentRound(_roundAttemptsCount);
        }
        
        
        private void InitCurrentRound(int attemptsCount)
        {
            var roundType = _chapter.levelData.levels[_battleManager.battle.roundIndex].roundType;
            if (attemptsCount > 0)
            {
                GrantPlayerInput();
                return;
            }
            AllowPlayerUIInput(false);
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
            _devilsOffer.MakeNextOffer(ShowMerchantOffer);
        }

        private async void ShowMerchantOffer()
        {
            await Task.Delay((int)(_delayBetweenOffers * 1000));
            if (_token == null || _token.IsCancellationRequested) return;
            _merchantOffer.MakeNextOffer(GrantPlayerInput);
        }
        
        private void GrantPlayerInput()
        {
            _mergeManager.AllowInput(true);
            AllowPlayerUIInput(true);
            _battleCamera.AllowPlayerInput(true);
        }

        private void AllowPlayerUIInput(bool allow) => _mainUI.AllowButtonsInput(allow);

        private void ShowFailedScreen()
        {
            _mergeManager.AllowInput(false);
            var lvlFail = ServiceLocator.Get<IUIManager>().Show<InvasionLevelFailUI>(UIConstants.UILevelFail, () =>{});
            lvlFail.Show(Replay, ReturnToMenu);
        }
        
        private bool SubtractHealthAndCheckLoose()
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

        private void AddRewardForLevelAndShowUI()
        {
            var chapterConfig = ServiceLocator.Get<ProgressionDataBase>().chapters[_selectionData.chapterIndex];
            var playerData = DataHelpers.GetPlayerData();
            ProgressionManager.CompleteTier(_selectionData.chapterIndex, _selectionData.tierIndex, out var completedFirstTime);

            var rewardMultiplier = _selectionData.multiplierTier * _selectionData.tierRewardMultiplier;
            var goldReward = Mathf.RoundToInt(rewardMultiplier * chapterConfig.moneyGoldReward);
            var xpReward = Mathf.RoundToInt(rewardMultiplier * chapterConfig.playerXpReward);

            var addedHeroesXp = Mathf.RoundToInt(rewardMultiplier * chapterConfig.heroXpReward);
            var heroesSaves = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
            foreach (var id in playerData.party.heroesIds)
                heroesSaves.GetSave(id).xp += addedHeroesXp;

            var gm = ServiceLocator.Get<GameMoney>();
            gm.globalMoney.AddValue(goldReward);
            
            var newPlayerXp = ServiceLocator.Get<CastleXpManager>().AddXp(xpReward);
            var outputRewards = new List<CoreItemData>(5);
            
            outputRewards.Add(new CoreItemData(goldReward, "gold", "bonus"));
            outputRewards.Add(new CoreItemData(goldReward, "player_xp", "bonus"));
            if (completedFirstTime)
            {
                AddRew(outputRewards, chapterConfig.tiers[_selectionData.tierIndex].additionalRewards);
                if (_selectionData.tierIndex == chapterConfig.tiers.Count - 1)
                {
                    AddRew(outputRewards, chapterConfig.chapterCompletedRewards);
                }
            }
            if (_logRewardItems)
            {
                var msg = "Added rewards after level win!";
                foreach (var it in outputRewards)
                    msg += $"\n{it.id}  {it.level}";
                CLog.Log(msg);          
            }

            var heroesCount = playerData.party.maxCount;
            var addedXps = new List<float>(heroesCount) { addedHeroesXp, addedHeroesXp, addedHeroesXp, addedHeroesXp, addedHeroesXp, addedHeroesXp};
            var ui = ServiceLocator.Get<IUIManager>().Show<InvasionLevelWinUI>(UIConstants.UILevelWin, () => {});
            ui.Show(new InvasionWinArgs()
            {
                playerNewLevelReached = newPlayerXp,
                playerXpAdded = xpReward,
                heroesXp = addedXps,
                selectionData = _selectionData,
                rewards = outputRewards,
                replayCallback = Replay,
                returnCallback = ReturnToMenu
            });
        }

        private void AddRew(List<CoreItemData> outputRewards,  List<CoreItemData> configRewards)
        {
            var playerData = DataHelpers.GetPlayerData();
            foreach (var reward in configRewards)
            {
                var upgReward = new CoreItemData(reward);
                upgReward.level = Mathf.RoundToInt(upgReward.level);
                outputRewards.Add(upgReward);
                switch (reward.type)
                {
                    case ItemsIds.TypeItem:
                        playerData.inventory.AddItem(reward.id, reward.level);
                        break;
                    case ItemsIds.TypeRelic:
                        RelicsManager.AddRelic(playerData.relics, reward.id);
                        break;
                    default:
                        CLog.LogRed($"reward type: \"{reward.type}\" unknown will not add to inventory!");
                        break;
                }
            }
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
            ServiceLocator.Get<PlayerEnergyManager>().Subtract(_selectionData.totalEnergyCost);
            SceneManager.LoadScene(GlobalConfig.SceneBattle);
        }
   
    }
}