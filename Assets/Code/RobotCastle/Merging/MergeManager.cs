using System;
using System.Collections.Generic;
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
        [SerializeField] private MergeGridHighlighter _highlighter;
        [SerializeField] private MergeCellHighlightPool _mergeCellHighlight;
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
            _gridView = _gridViewGo.GetComponent<IGridView>();
            _grid = _gridView.BuildGridFromView();
            CLog.Log($"[{nameof(MergeManager)}] Building grid view");
            _itemsSpawner = gameObject.GetComponent<IGridItemsSpawner>();
            _mergeProcessor = new ClassBasedMergeProcessor();
            _mergeController = new MergeController(_grid, _mergeProcessor,_itemsSpawner, _cellAvailabilityController, _gridView);
            _mergeInput = gameObject.GetComponent<MergeInput>();
            _mergeInput.Init(_mergeController);
            if(_allowInputOnStart)
                _mergeInput.SetActive(true);
            
            // Change this!
            const int maxCount = 3;
            _cellAvailabilityController.SetMaxCount(maxCount);
            ServiceLocator.Bind<ICellAvailabilityController>(_cellAvailabilityController);
            ServiceLocator.Bind<MergeController>(_mergeController);
            ServiceLocator.Bind<IGridItemsSpawner>(_itemsSpawner);
            ServiceLocator.Bind<MergeManager>(this);
            ServiceLocator.Bind<MergeCellHighlightPool>(_mergeCellHighlight);
            ServiceLocator.Bind<IMergeCellHighlightPool>(_mergeCellHighlight);
            _mergeCellHighlight.Init();
            _highlighter.Init(_gridView, _mergeProcessor);
            _mergeController.OnPutItem += OnItemPut;
            _mergeController.OnItemPicked += OnItemPicked;
            ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => {}).ShowIdle();
        }

        public void SetInputActive(bool active) => _mergeInput.SetActive(active);

        public bool SpawnHero(string id)
        {
            var controller = ServiceLocator.Get<MergeController>();
            if (controller.GetFreeCellForNewHero(out var view))
            {
                var spawner = ServiceLocator.Get<IGridItemsSpawner>();
                spawner.SpawnItemOnCell(view, id);
                HighlightMergeOptions();
                return true;
            }
            return false;
        }

        public bool SpawnHero(CoreItemData coreData)
        {
            if (_mergeController.GetFreeCellForNewHero(out var view))
            {
                var spawner = ServiceLocator.Get<IGridItemsSpawner>();
                spawner.SpawnItemOnCell(view, new ItemData(coreData.level, coreData.id, coreData.type));
                HighlightMergeOptions();
                return true;
            }
            return false;
        }
        

        [ContextMenu("LogGridState")]
        public void LogGridState()
        {
            var msg = _grid.GetStateAsStr();
            CLog.LogGreen("=== Grid State ===");
            CLog.LogWhite(msg);
        }

        private void HighlightMergeOptions()
        {
            CLog.LogWhite($"[{nameof(MergeManager)}] Highlight");
            var allItems = GetAllItemsOnGrid();
            _highlighter.HighlightAllPotentialCombinations(allItems);
        }
        
        private void OnItemPicked(ItemData srcItem)
        {
            var allItems = GetAllItemsOnGrid();
            _highlighter.HighlightForSpecificItem(allItems, srcItem);
        }
        
        private void OnItemPut(MergePutResult result)
        {
            HighlightMergeOptions();
        }

        private List<ItemData> GetAllItemsOnGrid()
        {
            var allItems = new List<ItemData>(20);
            foreach (var row in _grid.rows)
            {
                foreach (var cell in row.cells)
                {
                    if(cell.currentItem.IsEmpty() == false)
                        allItems.Add(cell.currentItem);
                }
            }
            return allItems;
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