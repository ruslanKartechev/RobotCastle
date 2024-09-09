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
        [SerializeField] private MergeManager _mergeManager;
        [SerializeField] private BattleManager _battleManager;
        
        private void Start()
        {
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            InitUI();
            yield return null;
            _mergeManager.Init();
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