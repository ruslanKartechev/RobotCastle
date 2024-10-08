using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using RobotCastle.Battling.DevilsOffer;
using RobotCastle.Battling.SmeltingOffer;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(35)]
    public class BattleLevel : MonoBehaviour, IBattleStartedProcessor, IBattleEndProcessor
    {
        [SerializeField] private int _startMoney = 0;
        [SerializeField] private bool _fillActiveAreaBeforeStart = true;
        [SerializeField] private float _endDelay = 2f;
        [SerializeField] private bool _doAutoBegin = true;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private MapBuilder _map;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private BattleCamera _battleCamera;
        [SerializeField] private BattleGridSwitch _gridSwitch;
        [SerializeField] private CastleHealthView _playerHealthView;
        [SerializeField] private ParticleSystem _troopSizeExpansion;
        [SerializeField] private SmeltingConfigContainer _smeltingConfigContainer;
        [SerializeField] private DevilsOfferConfigContainer _devilsOfferConfigContainer;
        private IPlayerMergeItemPurchaser _itemPurchaser;
        private ITroopSizeManager _troopSizeManager;
        private BattleMergeUI _mainUI;
        private Chapter _chapter;
        private CancellationTokenSource _token;
        private SmeltingOfferManager _smeltingOffer;
        private DevilsOfferManager _devilsOffer;
        
        
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

        #if UNITY_EDITOR
        [ContextMenu("ForceShowSmeltingOffer")]
        public void ForceShowSmeltingOffer()
        {
            AllowPlayerUIInput(false);
            ShowSmeltingOffer();
        }

        public void ForceShowTradeOffer()
        {
            
        }

        [ContextMenu("ForceShowDevilOffer")]
        public void ForceShowDevilOffer()
        {
            AllowPlayerUIInput(false);
            ShowDevilsOffer();
        }
        #endif
        
        private void Start()
        {
            GameState.Mode = GameState.EGameMode.InvasionBattle;
            var save = DataHelpers.GetInvasionProgress();
            _chapter = ServiceLocator.Get<ProgressionDataBase>().chapters[save.chapterIndex];
            _token = new CancellationTokenSource();
            ServiceLocator.Get<GameMoney>().levelMoney = _startMoney;
            try
            {
                Init(_token.Token);
            }catch(System.Exception ex)
            {
                CLog.Log(ex.Message);    
            }
        }

        private async Task Init(CancellationToken token)
        {
            _map.InitRuntime();
            ServiceLocator.Bind<IMap>(_map.Map);
            ServiceLocator.Bind<BattleCamera>(_battleCamera);
            ServiceLocator.Get<IUIManager>().ParentCanvas = _mainCanvas;
            ServiceLocator.Bind<EnemiesManager>(_enemiesManager);
            ServiceLocator.Bind<CastleHealthView>(_playerHealthView);
            ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });

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
            _devilsOffer = new DevilsOfferManager(_devilsOfferConfigContainer.config, _enemiesManager, _mergeManager.GridView, _mergeManager.SectionsController, _troopSizeManager, _battleManager, _playerHealthView);
            try
            {
                await _battleManager.SetStage(0, token);
            }
            catch (System.Exception ex)
            {
                CLog.LogError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
            _battleCamera.AllowPlayerInput(false);
            _battleCamera.SetBattlePoint();
            var config = ServiceLocator.Get<GlobalConfig>();
            await Task.Delay((int)(config.BattleStartEnemyShowTime * 1000), token);
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
            if (_battleManager.CanStart())
            {
                if(_fillActiveAreaBeforeStart)
                    _mergeManager.FillActiveArea();
                StartingBattle(_token.Token);
            }
        }

        private async Task StartingBattle(CancellationToken token)
        {
            _battleCamera.AllowPlayerInput(false);
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

        private void ShowFailedScreen()
        {
            _mergeManager.AllowInput(false);
            ServiceLocator.Get<IUIManager>().Show<LevelFailUI>(UIConstants.UILevelFail, () =>{});
        }
        
        private async Task BattleRoundCompletion(Battle battle, CancellationToken token)
        {
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
                    ShowDevilsOffer();
                    break;
                case RoundType.Boss:
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
                ShowFailedScreen();
                return true;
            }
            return false;
        }
        
    }
}