using System;
using System.Collections;
using Bomber;
using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(35)]
    public class BattleSceneManager : MonoBehaviour
    {
        [SerializeField] private bool _doAutoBegin = true;
        [SerializeField] private string _enemiesPreset;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private MapBuilder _map;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private BattleCamera _battleCamera;
        
        
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
            _battleManager.Init();
            InitUI();
            yield return null;
            _mergeManager.Init();
            _enemiesManager.Init();
            yield return null;
            if (_doAutoBegin)
                _battleManager.SetStage(0);
        }
        
        private void InitUI()
        {
            var ui = ServiceLocator.Get<IUIManager>(); 
            ui.Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
            ui.Show<BattleMergeUI>(UIConstants.UIBattleMerge, () => { });
        }
    }
}