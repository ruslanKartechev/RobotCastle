using System;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    [DefaultExecutionOrder(30)]
    public class MergeManager : MonoBehaviour
    {
        [SerializeField] private bool _autoInit;
        [SerializeField] private bool _allowInputOnStart;
        [SerializeField] private GameObject _gridViewGo;
        [SerializeField] private CellAvailabilityControllerBySection _cellAvailabilityController;
        private IGridView _gridView;
        private IMergeProcessor _mergeProcessor;
        private MergeController _mergeController;
        private MergeInput _mergeInput;
        private IGridItemsSpawner _itemsSpawner;
        private MergeGrid _grid;
        private bool _didInit;

        public void Init()
        {
            if (_didInit)
            {
                CLog.LogRed($"[MergeManager] Already did init! Possible error");
                return;
            }
            _didInit = true;
            // _gridData = ServiceLocator.Get<IDataSaver>().GetData<MergeGrid>();
            _gridView = _gridViewGo.GetComponent<IGridView>();
            _grid = _gridView.BuildGridFromView();
            CLog.Log($"[{nameof(MergeManager)}] Building grid view");
            _itemsSpawner = gameObject.GetComponent<IGridItemsSpawner>();
            // _itemsSpawner.SpawnItemsForGrid(_gridView, _grid);
            CLog.Log($"[{nameof(MergeManager)}] Spawning Items");
            
            _mergeProcessor = new ClassBasedMergeProcessor();
            _mergeController = new MergeController(_grid, _mergeProcessor,_itemsSpawner, _cellAvailabilityController, _gridView);
            _mergeInput = gameObject.GetComponent<MergeInput>();
            _mergeInput.Init(_mergeController);
            if(_allowInputOnStart)
                _mergeInput.SetActive(true);
            
            // Change this!
            const int maxCount = 3;
            _cellAvailabilityController.SetMaxCount(maxCount);
            ServiceLocator.Bind<MergeController>(_mergeController);
            ServiceLocator.Bind<IGridItemsSpawner>(_itemsSpawner);
            ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
        }

        public void SetInputActive(bool active) => _mergeInput.SetActive(active);

        [ContextMenu("LogGridState")]
        public void LogGridState()
        {
            var msg = _grid.GetStateAsStr();
            CLog.LogGreen("=== Grid State ===");
            CLog.LogWhite(msg);
        }
        
        
        private void Start()
        {
            if(_autoInit)
                Init();
        }
        
        private void OnDestroy()
        {
            ServiceLocator.Unbind<MergeController>();
        }
    }
    
}