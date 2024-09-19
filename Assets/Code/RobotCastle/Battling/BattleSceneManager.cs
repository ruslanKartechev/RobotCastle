using System;
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
    public class BattleSceneManager : MonoBehaviour, IBattleStartedProcessor, IBattleEndProcessor
    {
        [SerializeField] private bool _doAutoBegin = true;
        [SerializeField] private PresetsContainer _presetsContainer;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private MapBuilder _map;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private BattleCamera _battleCamera;
        [SerializeField] private GameObject _cellsParent;
        
        
        public void OnBattleStarted(Battle battle)
        {
            _cellsParent.gameObject.SetActive(false);
            _mergeManager.AllowInput(false);
            _battleCamera.MoveToBattlePoint();
        }

        public void OnBattleEnded(Battle battle)
        {
            _cellsParent.gameObject.SetActive(true);
            _battleCamera.MoveToMergePoint();
            _mergeManager.AllowInput(true);
            if (battle.State == BattleState.EnemyWin)
            {
                _battleManager.ResetStage();
            }
            else if (battle.State == BattleState.PlayerWin)
            {
                _battleManager.NextStage();
            }
            else
            {
                CLog.LogRed($"[{nameof(BattleSceneManager)}] Error!");
            }
        }
        
        private void Start()
        {
            _map.InitRuntime();
            ServiceLocator.Bind<IMap>(_map.Map);
            ServiceLocator.Bind<BattleCamera>(_battleCamera);
            ServiceLocator.Get<IUIManager>().ParentCanvas = _mainCanvas;
            ServiceLocator.Bind<EnemiesManager>(_enemiesManager);
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            _battleManager.Init(_presetsContainer);
            InitUI();
            yield return null;
            _mergeManager.Init();
            _enemiesManager.Init();
            yield return null;
            if (_doAutoBegin)
                _battleManager.SetStage(0);
            _mergeManager.AllowInput(true);
        }
        
        private void InitUI()
        {
            var ui = ServiceLocator.Get<IUIManager>(); 
            ui.Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
            ui.Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
        }
        
    }
}