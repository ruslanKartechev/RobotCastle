using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(35)]
    public class BattleLevel : MonoBehaviour, IBattleStartedProcessor, IBattleEndProcessor
    {
        [SerializeField] private bool _fillActiveAreaBeforeStart = true;
        [SerializeField] private float _endDelay = 2f;
        [SerializeField] private bool _doAutoBegin = true;
        [SerializeField] private InvasionLevelDataContainer _levelContainer;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private MapBuilder _map;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private BattleCamera _battleCamera;
        [SerializeField] private BattleGridSwitch _gridSwitch;
        [SerializeField] private CastleHealthView _playerHealthView;
        [SerializeField] private ParticleSystem _troopSizeExpansion;
        private IPlayerMergeItemPurchaser _itemPurchaser;
        private ITroopSizeManager _troopSizeManager;
        private BattleMergeUI _mainUI;
        private CancellationTokenSource _token;
        private InvasionLevelData levelData => _levelContainer.data;
        private InvasionRoundData roundData => levelData.levels[_battleManager.battle.stageIndex];
        
        
        public void OnBattleStarted(Battle battle)
        {
            AllowPlayerUIInput(true);
            _gridSwitch.SetBattleMode();
            _mergeManager.AllowInput(false);
            ServiceLocator.Get<MergeManager>().StopHighlight();
        }

        public async void OnBattleEnded(Battle battle)
        {
            BattleCompletion(battle, _token.Token);
        }
        
        private void Start()
        {
            _token = new CancellationTokenSource();
            Init(_token.Token);
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
            var battle = _battleManager.Init(levelData.levels);
            _troopSizeManager = new BattleTroopSizeManager(battle, _troopSizeExpansion);
            ServiceLocator.Bind<ITroopSizeManager>(_troopSizeManager);
            _mergeManager.Init();
            _enemiesManager.Init();
            _playerHealthView.SetHealth(battle.playerHealthPoints);
            _itemPurchaser = gameObject.GetComponent<IPlayerMergeItemPurchaser>();
            await _battleManager.SetStage(0, token);
            _battleCamera.AllowPlayerInput(false);
            _battleCamera.SetBattlePoint();
            var config = ServiceLocator.Get<GlobalConfig>();
            await Task.Delay((int)(config.BattleStartEnemyShowTime * 1000), token);
            InitUI();
            // await Task.Delay((int)(config.BattleStartInputDelay * 1000));
            await _battleCamera.MoveToMergePoint();
            _mergeManager.AllowInput(true);
            _battleCamera.AllowPlayerInput(true);
        }
        
        private void InitUI()
        {
            var uiManager = ServiceLocator.Get<IUIManager>();
            uiManager.Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
            var dam = uiManager.Show<BattleDamagePanelUI>("ui_damage", () => { });
            dam.gameObject.SetActive(true);
            
            _mainUI = uiManager.Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            _mainUI.BtnSpawn.AddMainCallback(BuyNewItem);
            _mainUI.BtnStart.AddMainCallback(StartBattle);
            _mainUI.BtnStart2.AddMainCallback(StartBattle);
            _mainUI.TroopSizePurchaseUI.Init(_troopSizeManager);
            _mainUI.TroopSizePurchaseUI.SetInteractable(true);
            _mainUI.Init(_battleManager.battle, levelData);
            AllowPlayerUIInput(true);
        }

        private void AllowPlayerUIInput(bool allow)
        {
            _mainUI.BtnSpawn.SetInteractable(allow);
            _mainUI.BtnStart.SetInteractable(allow);
            _mainUI.BtnStart2.SetInteractable(allow);
        }
        
        private void StartBattle()
        {
            if (_battleManager.CanStart())
            {
                if(_fillActiveAreaBeforeStart)
                    _mergeManager.FillActiveArea();
                StartCoroutine(Starting());
            }
        }

        private IEnumerator Starting()
        {
            _battleCamera.AllowPlayerInput(false);
            _battleManager.SetGoingState();
            _battleCamera.MoveToBattlePoint();
            _mainUI.Started();
            foreach (var hero in _battleManager.battle.playersAlive)
                hero.View.transform.rotation = Quaternion.Euler(Vector3.zero);
            yield return new WaitForSeconds(_battleCamera.MoveTime);
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
        
        private async Task BattleCompletion(Battle battle, CancellationToken token)
        {
            _battleManager.BattleRewardCalculator.AddRewardForStage();
            await Task.Delay((int)(_endDelay * .5f * 1000), token);
            _gridSwitch.SetMergeMode();
            _battleManager.RecollectPlayerUnits();
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
            await Task.Delay((int)(_endDelay * .5f * 1000), token);

            var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
            if(battleUI)
                battleUI.SetMainAreaUpPos();
            await _battleCamera.MoveToMergePoint();
            _mergeManager.AllowInput(true);
            AllowPlayerUIInput(true);
            _battleCamera.AllowPlayerInput(true);
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