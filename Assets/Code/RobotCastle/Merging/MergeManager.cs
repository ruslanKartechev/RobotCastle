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
        [SerializeField] private GridSectionsController _sectionsController;
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
            _mergeController = new MergeController(_grid, _mergeProcessor,_itemsSpawner, _sectionsController, _gridView);
            _mergeInput = gameObject.GetComponent<MergeInput>();
            _mergeInput.Init(_mergeController);
            if(_allowInputOnStart)
                _mergeInput.SetActive(true);
            
            // Change this!
            const int maxCount = 3;
            _sectionsController.SetMaxCount(maxCount);
            _sectionsController.Init(_gridView);
            ServiceLocator.Bind<IGridSectionsController>(_sectionsController);
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
                var itemView = spawner.SpawnItemOnCell(view, id);
                _sectionsController.OnItemPut(itemView.itemData);
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
                var itemView = spawner.SpawnItemOnCell(view, new ItemData(coreData.level, coreData.id, coreData.type));
                _sectionsController.OnItemPut(itemView.itemData);
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

        public void MergeAll()
        {
            CLog.Log($"[{nameof(MergeManager)}] Merge All");
            var items = _sectionsController.GetAllItemViewsInMergeArea();
            var merged = _mergeProcessor.MergeAllItemsPossible(items, _gridView);
            _mergeProcessor.SortAllItemsPossible(merged, _gridView);
            HighlightMergeOptions();
        }

        public void SortAll()
        {
            CLog.Log($"[{nameof(MergeManager)}] Sort All");   
            var items = _sectionsController.GetAllItemViewsInMergeArea();
            _mergeProcessor.SortAllItemsPossible(items, _gridView);
            HighlightMergeOptions();
        }

        private void HighlightMergeOptions()
        {
            CLog.LogWhite($"[{nameof(MergeManager)}] Highlight");
            var allItems = _sectionsController.GetAllItems();
            _highlighter.HighlightAllPotentialCombinations(allItems);
        }
        
        private void OnItemPicked(ItemData srcItem)
        {
            var allItems = _sectionsController.GetAllItems();
            _highlighter.HighlightForSpecificItem(allItems, srcItem);
        }
        
        private void OnItemPut(MergePutResult result)
        {
            HighlightMergeOptions();
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