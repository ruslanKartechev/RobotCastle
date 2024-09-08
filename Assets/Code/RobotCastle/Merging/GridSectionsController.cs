using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class GridSectionsController : MonoBehaviour, IGridSectionsController
    {
        [SerializeField] private int _minYIndex = 2;
        [SerializeField] private int _maxCount = 3;
        private IGridView _gridView;
        private int _currentCount = 0;

        public int MaxCount => _maxCount;

        public void Init(IGridView gridView)
        {
            _gridView = gridView;
        }

        public void SetMaxCount(int maxCount)
        {
            _maxCount = maxCount;
            if (ServiceLocator.GetIfContains(out ITroopsCountView troopsCountView))
                troopsCountView.UpdateCount(_currentCount, _maxCount);
            else
                CLog.Log("ITroopsCountView not setup yet!");
        }
        
        public bool IsCellAllowed(int x, int y, ItemData item, bool promptUser = true)
        {
            if (y < _minYIndex) // moving to lower region
                return true;
            if (item.pivotY >= _minYIndex && y >= _minYIndex) // already in the upper region and moving to upper region
                return true;
            if (item.core.type != MergeConstants.TypeUnits && y >= _minYIndex) // moving non-unit to upper zone
            {
                return false;
            }
            
            var allowed = _currentCount < _maxCount;
            if (promptUser && !allowed)
            {
                var ui = ServiceLocator.Get<IUIManager>().Show<IMergeInfoUI>(UIConstants.UIMergeInfo, () => {});
                ui.ShowNotEnoughTroopSize(_currentCount, _maxCount);
            }
            return _currentCount < _maxCount;
        }

        public void OnGridUpdated(MergeGrid grid)
        {
            var count = 0;
            for (var y = _minYIndex; y < grid.rows.Count; y++)
                count += grid.rows[y].CalculateItemsCount();
            if (count != _currentCount)
            {
                _currentCount = count;
                if (ServiceLocator.GetIfContains(out ITroopsCountView troopsCountView))
                    troopsCountView.UpdateCount(_currentCount, _maxCount);           
            }
        }

        public void OnItemPut(ItemData item)
        {
            OnGridUpdated(_gridView.BuiltGrid);
            var cell = _gridView.GetCell(item.pivotX, item.pivotY);
            if (!cell.cell.isOccupied)
                return;
            var itemView = cell.itemView;
            if (item.core.type == MergeConstants.TypeUnits)
            {
                var ui = itemView.Transform.GetComponent<UnitView>().UnitUI;
                if(item.pivotY >= _minYIndex)
                    ui.SetBattleMode();
                else
                    ui.SetMergeMode();              
            }
        }

        public bool GetFreeCell(MergeGrid grid, out Vector2Int coordinates)
        {
            for (var y = _minYIndex-1; y >= 0; y--)
            {
                var row = grid.rows[y].cells;
                for (var x = 0; x < row.Count; x++)
                {
                    if (row[x].isUnlocked && row[x].isOccupied == false)
                    {
                        coordinates = new Vector2Int(x, y);
                        return true;
                    }
                }
            }
            coordinates = new Vector2Int();
            return false;
        }

        public int GetFreeCellsCount(MergeGrid grid)
        {
            var count = 0;
            for (var y = _minYIndex-1; y >= 0; y--)
            {
                var row = grid.rows[y].cells;
                for (var x = 0; x < row.Count; x++)
                {
                    if (row[x].isUnlocked && row[x].isOccupied == false)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public List<ItemData> GetAllItems()
        {
            var allItems = new List<ItemData>(20);
            var grid = _gridView.BuiltGrid;
            foreach (var row in grid.rows)
            {
                foreach (var cell in row.cells)
                {
                    if(cell.currentItem.IsEmpty() == false)
                        allItems.Add(cell.currentItem);
                }
            }
            return allItems;
        }

        public List<ItemData> GetAllItemsInMergeArea()
        {
            var allItems = new List<ItemData>(10);
            var grid = _gridView.BuiltGrid;
            for (var y = 0; y <= _minYIndex; y++)
            {
                var row = grid.rows[y];
                foreach (var cell in row.cells)
                {
                    if (cell.currentItem.IsEmpty() == false)
                        allItems.Add(cell.currentItem);
                }
            }
            return allItems;
        }

        public List<IItemView> GetAllItemViewsInMergeArea()
        {
            var allItems = new List<IItemView>(10);
            for (var y = 0; y <= _minYIndex; y++)
            {
                var maxX = _gridView.Grid.GetLength(0);
                for (var x = 0; x < maxX; x++)
                {
                    var cell = _gridView.Grid[x, y];
                    if(cell.itemView != null && !cell.itemView.itemData.IsEmpty())
                        allItems.Add(cell.itemView);
                }
            }
            return allItems;
        }
        
        private void Start()
        {
            if (ServiceLocator.GetIfContains(out ITroopsCountView troopsCountView))
                troopsCountView.UpdateCount(_currentCount, _maxCount);
            else
                CLog.Log("ITroopsCountView not setup yet!");
        }
    }
}