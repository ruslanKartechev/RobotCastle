using System.Collections;
using Bomber;
using RobotCastle.Core;
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
        [SerializeField] private PresetsContainer _presetsContainer;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private MapBuilder _map;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private BattleCamera _battleCamera;
        [SerializeField] private GameObject _cellsParent;
        private IPlayerMergeItemPurchaser _itemPurchaser;
        private ITroopSizeManager _troopSizeManager;
        
        public void OnBattleStarted(Battle battle)
        {
            AllowPlayerUIInput(false);
            _cellsParent.gameObject.SetActive(false);
            _mergeManager.AllowInput(false);
            _battleCamera.MoveToBattlePoint();
            ServiceLocator.Get<MergeManager>().StopHighlight();
        }

        public void OnBattleEnded(Battle battle)
        {
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
            
            _itemPurchaser = gameObject.GetComponent<IPlayerMergeItemPurchaser>();
            _battleManager.Init(_presetsContainer);
            _battleManager.startProcessor = this;
            _battleManager.endProcessor = this;
            _troopSizeManager = new BattleTroopSizeManager(_battleManager.battle);
            ServiceLocator.Bind<ITroopSizeManager>(_troopSizeManager);
            InitUI();
            yield return null;
            _mergeManager.Init();
            _enemiesManager.Init();
            yield return null;
            if (_doAutoBegin)
                _battleManager.SetStage(0);
            yield return null;
            _battleCamera.AllowPlayerInput(false);
            _battleCamera.SetBattlePoint();
            var config = ServiceLocator.Get<GlobalConfig>();
            yield return new WaitForSeconds(config.BattleStartEnemyShowTime);
            _battleCamera.MoveToMergePoint();
            yield return new WaitForSeconds(config.BattleStartInputDelay);
            _mergeManager.AllowInput(true);
            _battleCamera.AllowPlayerInput(true);
        }
        
        private void InitUI()
        {
            var ui = ServiceLocator.Get<IUIManager>();
            ui.Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
            var dam = ui.Show<BattleDamagePanelUI>("ui_damage", () => { });
            dam.gameObject.SetActive(true);
            
            var mergeUI = ui.Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            mergeUI.BtnSpawn.AddMainCallback(BuyNewItem);
            mergeUI.BtnStart.AddMainCallback(StartBattle);
            mergeUI.TroopSizePurchaseUI.Init(_troopSizeManager);
            mergeUI.TroopSizePurchaseUI.SetInteractable(true);
            AllowPlayerUIInput(true);
        }

        private void AllowPlayerUIInput(bool allow)
        {
            var ui = ServiceLocator.Get<IUIManager>().Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
            ui.BtnSpawn.SetInteractable(allow);
            ui.BtnStart.SetInteractable(allow);
        }
        
        private void StartBattle()
        {
            _battleCamera.AllowPlayerInput(false);
            if(_fillActiveAreaBeforeStart)
                _mergeManager.FillActiveArea();
            _battleManager.BeginBattle();
        }

        private void BuyNewItem()
        {
            _itemPurchaser.TryPurchaseItem();
        }
        
        private IEnumerator DelayedBattleEnd(Battle battle)
        {
            yield return new WaitForSeconds(_endDelay / 2f);
            _cellsParent.gameObject.SetActive(true);
            _battleManager.RecollectPlayerUnits();
            if (battle.State == BattleState.EnemyWin)
                _battleManager.ResetStage();
            else if (battle.State == BattleState.PlayerWin)
                _battleManager.NextStage();
            else
                CLog.LogRed($"[{nameof(BattleLevel)}] Error!");
            
            yield return new WaitForSeconds(_endDelay / 2f);
            _battleCamera.MoveToMergePoint();
            _mergeManager.AllowInput(true);
            AllowPlayerUIInput(true);
            _battleCamera.AllowPlayerInput(true);
        }
        
    }
}