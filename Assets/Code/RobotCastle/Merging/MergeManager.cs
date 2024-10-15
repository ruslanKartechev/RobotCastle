using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    [DefaultExecutionOrder(30)]
    public class MergeManager : MonoBehaviour
    {
        public IGridView GridView => _gridView;
        public IGridSectionsController SectionsController => _sectionsController;
        public IMergeProcessor MergeProcessor => _mergeProcessor;
        public IMergeItemsContainer Container => _itemsContainer;
            
        [SerializeField] private bool _autoInit;
        [SerializeField] private bool _allowInputOnStart;
        [SerializeField] private GameObject _gridViewGo;
        [SerializeField] private MergeGridHighlighter _highlighter;
        [SerializeField] private SimplePoolsManager _mergeCellHighlight;
        
        private IGridSectionsController _sectionsController;
        private IGridView _gridView;
        private IMergeProcessor _mergeProcessor;
        private IMergeItemsFactory _itemsSpawner;
        private IMergeItemsContainer _itemsContainer;
        private IMergeMaxLevelCheck _maxLevelCheck;
        private MergeController _mergeController;
        private MergeInput _mergeInput;
        private GridViewsContainer _viewsContainer;
        private MergeGrid _grid;
        private bool _didInit;

        //REMOVE THIS
        [ContextMenu("LogAllItems")]
        public void LogAllItems()
        {
            var msg = "All Player Items: ";
            foreach (var itemView in _itemsContainer.allItems)
            {
                msg += $" {itemView.itemData.core.id}, ";
            }
            CLog.LogWhite(msg);
        }
        
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
            _itemsSpawner = gameObject.GetComponent<IMergeItemsFactory>();
            _sectionsController = gameObject.GetComponent<IGridSectionsController>();
            _mergeInput = gameObject.GetComponent<MergeInput>();
            _maxLevelCheck = new MergeMaxLevelCheck();
            _itemsContainer = new PlayerMergeItemsContainer();
            _mergeProcessor = new ClassBasedMergeProcessor(_itemsContainer, _maxLevelCheck);
            // _mergeProcessor.AddModifier(this);
            _mergeController = new MergeController(_mergeProcessor, _sectionsController, _gridView, new MergeSwapAllowedCheck(), _itemsContainer);
            _mergeInput.Init(_mergeController);
            _sectionsController.SetGridView(_gridView);
            _mergeCellHighlight.Init();
            _highlighter.Init(_gridView, _mergeProcessor);
            _mergeController.OnPutItem += OnItemPut;
            _mergeController.OnItemPicked += OnItemPicked;
            _viewsContainer = new GridViewsContainer();
            _viewsContainer.AddGridView(_gridView);
            BindRefs();
            if(_allowInputOnStart)
                _mergeInput.SetActive(true);
        }

        public void AllowInput(bool active) => _mergeInput.SetActive(active);

        public void FillActiveArea()
        {
            var filler = new MergeActiveAreaFiller(_gridView, _sectionsController);
            filler.Fill();
        }
        
        public bool SpawnNewMergeItem(SpawnMergeItemArgs args)
        {
            var didSpawn = ServiceLocator.Get<IHeroesAndItemsFactory>()
                .SpawnHeroOrItem(args, _gridView, _sectionsController, out var view);
            if (!didSpawn)
                return false;
            HighlightMergeOptions();
            return true;
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

        public void ResetAllItemsPositions()
        {
            foreach (var cell in _gridView.Grid)
            {
                if (cell.itemView != null)
                    MergeFunctions.SetItemGridAndWorldPosition(cell.itemView, cell);
            }
        }
        
        public void HighlightMergeOptions()
        {
            // CLog.Log($"[{nameof(MergeManager)}] Highlight potential merge");
            var allItems = _sectionsController.GetAllItems();
            _highlighter.HighlightAllPotentialCombinations(allItems);
        }

        public void StopHighlight()
        {
            _highlighter.StopHighlight();
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
        
        private void BindRefs()
        {
            ServiceLocator.Bind<IGridSectionsController>(_sectionsController);
            ServiceLocator.Bind<IMergeItemsFactory>(_itemsSpawner);
            ServiceLocator.Bind<ISimplePoolsManager>(_mergeCellHighlight);
            ServiceLocator.Bind<IMergeProcessor>(_mergeProcessor);
            ServiceLocator.Bind<MergeManager>(this);
            ServiceLocator.Bind<MergeController>(_mergeController);
            ServiceLocator.Bind<SimplePoolsManager>(_mergeCellHighlight);
            ServiceLocator.Bind<GridViewsContainer>(_viewsContainer);
            ServiceLocator.Bind<IHeroWeaponsContainer>(_itemsContainer);
            ServiceLocator.Bind<IMergeMaxLevelCheck>(_maxLevelCheck);
        }



        private void Start()
        {
            if(_autoInit)
                Init();
        }
        
        private void OnDestroy()
        {
            ServiceLocator.Unbind<IGridSectionsController>();
            ServiceLocator.Unbind<IMergeItemsFactory>();
            ServiceLocator.Unbind<ISimplePoolsManager>();
            ServiceLocator.Unbind<MergeController>();
            ServiceLocator.Unbind<MergeManager>();
            ServiceLocator.Unbind<SimplePoolsManager>();
            ServiceLocator.Unbind<GridViewsContainer>();
            ServiceLocator.Unbind<IMergeMaxLevelCheck>();
        }

    }
}