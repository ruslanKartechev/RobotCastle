using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
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
        private MergeGrid _grid;
        private int _currentCount = 0;

        public List<T> GetItemsInActiveArea<T>(MiscUtils.Condition<T> condition)
        {
            var units = new List<T>(_maxCount);
            for (var y = _minYIndex; y < _grid.RowsCount; y++)
            {
                var row = _grid.rows[y];
                for (var x = 0; x < row.Count; x++)
                {
                    // CLog.Log($"Checking {x}, {y}");
                    var cell = _gridView.GetCell(x, y);
                    if (cell.itemView != null)
                    {
                        // CLog.LogBlue($"{x}, {y} Has itemView");
                        var obj = cell.itemView.Transform.GetComponent<T>();
                        if(obj != null && condition.Invoke(obj))
                            units.Add(obj);
                    }
                }
            }
            return units;
        }

        public bool CanPutMoreIntoActiveZone() => _currentCount < _maxCount;

        public Vector2Int GetCoordinateForClosestCellInActiveZone(Vector2Int originalCell)
        {
            if (originalCell.y >= _minYIndex)
                return originalCell;
            for (var y = _minYIndex; y < _grid.RowsCount; y++)
            {
                var cell = _grid.GetCell(originalCell.x, y);
                if(cell.isUnlocked && !cell.isOccupied)
                    return new Vector2Int(originalCell.x, y);
            }

            var xMax = _grid.rows[0].cells.Count;
            var originalX = originalCell.x;
            for (var x = originalX; x < xMax; x++)
            {
                for (var y = _minYIndex; y < _grid.RowsCount; y++)
                {
                    var cell = _grid.GetCell(x, y);
                    if(cell.isUnlocked && !cell.isOccupied)
                        return new Vector2Int(x, y);
                }
            }
            
            for (var x = originalX; x >= 0; x--)
            {
                for (var y = _minYIndex; y < _grid.RowsCount; y++)
                {
                    var cell = _grid.GetCell(x, y);
                    if(cell.isUnlocked && !cell.isOccupied)
                        return new Vector2Int(x, y);
                }
            }
            return originalCell;
        }

        public void SetGridView(IGridView gridView)
        {
            _gridView = gridView;
            _grid = gridView.BuiltGrid;
        }

        public void SetMaxCount(int maxCount)
        {
            _maxCount = maxCount;
            if (ServiceLocator.GetIfContains(out ITroopsCountView view))
                view.UpdateCount(_currentCount, _maxCount);
            else
                CLog.Log("ITroopsCountView not setup yet!");
        }

        public bool IsCellAllowed(Vector2Int coord, ItemData item, bool promptUser = true)
        {
            if (coord.y < _minYIndex) // moving to lower region
                return true;
            if (item.pivotY >= _minYIndex && coord.y >= _minYIndex) // already in the upper region and moving to upper region
                return true;
            if (item.core.type != ItemsIds.TypeHeroes && coord.y >= _minYIndex) // moving non-unit to upper zone
                return false;
            
            var allowed = _currentCount < _maxCount;
            if (promptUser && !allowed)
            {
                var ui = ServiceLocator.Get<IUIManager>().Show<IMergeInfoUI>(UIConstants.UIMergeInfo, () => {});
                ui.ShowNotEnoughTroopSize(_currentCount, _maxCount);
            }
            return _currentCount < _maxCount;
        }

        public bool GetFreeAllowedCell(MergeGrid grid, ItemData itemData, out Vector2Int coordinates)
        {
            if (itemData.core.type == ItemsIds.TypeHeroes)
                return GetFreeCell(grid, out coordinates);
            for (var y = _minYIndex - 1; y >= 0; y--)
            {
                var (res, x) = SearchRow(grid.rows[y].cells);
                if (res)
                {
                    coordinates = new Vector2Int(x,y);
                    return true;
                }
            }
            coordinates = default;
            return false;
        }

        public void OnGridUpdated()
        {
            var count = 0;
            for (var y = _minYIndex; y < _grid.rows.Count; y++)
                count += _grid.rows[y].CalculateItemsCount();
            if (count != _currentCount)
            {
                _currentCount = count;
                if (ServiceLocator.GetIfContains(out ITroopsCountView troopsCountView))
                    troopsCountView.UpdateCount(_currentCount, _maxCount);           
            }
        }

        public void OnItemPut(ItemData item)
        {
            OnGridUpdated();
            var cell = _gridView.GetCell(item.pivotX, item.pivotY);
            if (!cell.cell.isOccupied)
                return;
            var itemView = cell.itemView;
            if (item.core.type == ItemsIds.TypeHeroes)
            {
                var ui = itemView.Transform.GetComponent<HeroComponents>().heroUI;
                if(item.pivotY >= _minYIndex)
                    ui.SetBattleMode();
                else
                    ui.SetMergeMode();              
            }
        }

        public bool GetFreeCell(MergeGrid grid, out Vector2Int coordinates)
        {
            if (_currentCount < _maxCount)
            {
                for (var y = _minYIndex; y < _grid.RowsCount; y++)
                {
                    var (res, x) = SearchRow(grid.rows[y].cells);
                    if (res)
                    {
                        coordinates = new Vector2Int(x,y);
                        return true;
                    }
                }
            }
            
            for (var y = _minYIndex - 1; y >= 0; y--)
            {
                var (res, x) = SearchRow(grid.rows[y].cells);
                if (res)
                {
                    coordinates = new Vector2Int(x,y);
                    return true;
                }
            }
            
            coordinates = new Vector2Int(-1,-1);
            return false;
        }
        
       private (bool, int) SearchRow(IList<Cell> row)
        {
            var count = row.Count;
            var num = 0;
            var center = count / 2 - 1;
            // if (center < 0)
            // center = 0;
            for (var x = center; num < count;)
            {
                if (row[x].isUnlocked && row[x].isOccupied == false)
                    return (true, x);
                x++;
                if (x >= count)
                    x = 0;
                num++;
            }
            return (false, -1);
        }   

        public bool IsCellFree(MergeGrid grid, ItemData itemData, Vector2Int coordinated)
        {
            var cell = grid.rows[coordinated.y].cells[coordinated.x];
            return cell.isUnlocked && !cell.isOccupied;
        }

        public int GetFreeCellsCount()
        {
            var count = 0;
            for (var y = _minYIndex - 1; y >= 0; y--)
            {
                var row = _grid.rows[y].cells;
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
            var count = grid.rows.Count;
            for (var r = count - 1; r >= 0; r--)
            {
                var row = grid.rows[r];
                foreach (var cell in row.cells)
                {
                    if (cell.currentItem.IsEmpty() == false)
                        allItems.Add(cell.currentItem);
                }
            }

            return allItems;
        }
        
        public List<IItemView> GetAllItemsViews()
        {
            var allItems = new List<IItemView>(20);
            var yMax = _gridView.Grid.GetLength(1);
            var xMax = _gridView.Grid.GetLength(0);
            for (var y = 0; y < yMax; y++)
            {
                for (var x = 0; x < xMax; x++)
                {
                    var cell = _gridView.Grid[x, y];
                    if (cell.itemView != null)
                        allItems.Add(cell.itemView);
                }
            }
            return allItems;
        }

        public List<ItemData> GetAllItemsInMergeArea()
        {
            var allItems = new List<ItemData>(10);
            var grid = _gridView.BuiltGrid;
            for (var y = 0; y < _minYIndex; y++)
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
            var maxX = _gridView.Grid.GetLength(0);
            for (var y = 0; y < _minYIndex; y++)
            {
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