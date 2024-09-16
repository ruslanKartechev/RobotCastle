using System;
using System.Collections;
using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(35)]
    public class BattleSceneManager : MonoBehaviour
    {
        [SerializeField] private string _enemiesPreset;
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private EnemiesManager _enemiesManager;
        [SerializeField] private Canvas _mainCanvas;
        
        private void Start()
        {
            ServiceLocator.Get<IUIManager>().ParentCanvas = _mainCanvas;
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            InitUI();
            yield return null;
            _mergeManager.Init();
            _enemiesManager.SpawnPreset(_enemiesPreset);
            yield return null;
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