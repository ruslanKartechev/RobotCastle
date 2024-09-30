using System.Collections;
using System.Runtime.CompilerServices;
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
        private InvasionLevelData levelData => _levelContainer.data;
        private InvasionRoundData roundData => levelData.levels[_battleManager.battle.stageIndex];
        
        
        public void OnBattleStarted(Battle battle)
        {
            AllowPlayerUIInput(false);
            _gridSwitch.SetBattleMode();
            _mergeManager.AllowInput(false);
            _battleCamera.MoveToBattlePoint();
            ServiceLocator.Get<MergeManager>().StopHighlight();
        }

        public void OnBattleEnded(Battle battle)
        {
            _battleManager.BattleRewardCalculator.AddRewardForStage();
            StartCoroutine(DelayedBattleEnd(battle));
        }
        
        private void Start()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            _map.InitRuntime();
            ServiceLocator.Bind<IMap>(_map.Map);
            ServiceLocator.Bind<BattleCamera>(_battleCamera);
            ServiceLocator.Get<IUIManager>().ParentCanvas = _mainCanvas;
            ServiceLocator.Bind<EnemiesManager>(_enemiesManager);
            ServiceLocator.Bind<CastleHealthView>(_playerHealthView);
            
            _itemPurchaser = gameObject.GetComponent<IPlayerMergeItemPurchaser>();
            _battleManager.startProcessor = this;
            _battleManager.endProcessor = this;
            var battle = _battleManager.Init(levelData.levels);
            _troopSizeManager = new BattleTroopSizeManager(battle, _troopSizeExpansion);
            ServiceLocator.Bind<ITroopSizeManager>(_troopSizeManager);
            _mergeManager.Init();
            _enemiesManager.Init();
            _playerHealthView.SetHealth(battle.playerHealthPoints);
            yield return null;
            if (_doAutoBegin)
                _battleManager.SetStage(0);
            _battleCamera.AllowPlayerInput(false);
            _battleCamera.SetBattlePoint();
            var config = ServiceLocator.Get<GlobalConfig>();
            yield return new WaitForSeconds(config.BattleStartEnemyShowTime);
            _battleCamera.MoveToMergePoint();
            InitUI();
            yield return new WaitForSeconds(config.BattleStartInputDelay);
            _mergeManager.AllowInput(true);
            _battleCamera.AllowPlayerInput(true);
        }
        
        private void InitUI()
        {
            var mergeInfoUI = ServiceLocator.Get<IUIManager>();
            mergeInfoUI.Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
            var dam = mergeInfoUI.Show<BattleDamagePanelUI>("ui_damage", () => { });
            dam.gameObject.SetActive(true);
            
            _mainUI = mergeInfoUI.Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
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
            _battleCamera.AllowPlayerInput(false);
            if(_fillActiveAreaBeforeStart)
                _mergeManager.FillActiveArea();
            _battleManager.BeginBattle();
            _mainUI.SetMainAreaLowerPos();
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
        
        private IEnumerator DelayedBattleEnd(Battle battle)
        {
            yield return new WaitForSeconds(_endDelay * .5f);
            _gridSwitch.SetMergeMode();
            _battleManager.RecollectPlayerUnits();
            switch (battle.State)
            {
                case BattleState.EnemyWin:
                    if(RoundLost())
                        yield break;
                    break;
                case BattleState.PlayerWin:
                       RoundWin();
                    break;
                default:
                    CLog.LogRed($"[{nameof(BattleLevel)}] Error!");
                    break;
            }
            yield return new WaitForSeconds(_endDelay * .5f);
            _battleCamera.MoveToMergePoint();
            var battleUI = ServiceLocator.Get<IUIManager>().GetIfShown<BattleMergeUI>(UIConstants.UIBattleMerge);
            if(battleUI)
                battleUI.SetMainAreaUpPos();
            _mergeManager.AllowInput(true);
            AllowPlayerUIInput(true);
            _battleCamera.AllowPlayerInput(true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RoundWin()
        {
            _battleManager.NextStage();
            _mainUI.UpdateForNextWave();
            _mainUI.Win();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RoundLost()
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
            _battleManager.ResetStage();
            return false;
        }
        
    }
    
}