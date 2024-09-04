using System;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class CellAvailabilityControllerByCount : MonoBehaviour, ICellAvailabilityController
    {
        [SerializeField] private int _minYIndex = 2;
        [SerializeField] private int _maxCount = 3;
        private int _currentCount = 0;

        public int MaxCount => _maxCount;

        private void Start()
        {
            if (ServiceLocator.GetIfContains(out ITroopsCountView troopsCountView))
                troopsCountView.UpdateCount(_currentCount, _maxCount);
            else
                CLog.Log("ITroopsCountView not setup yet!");
        }

        public void SetMaxCount(int maxCount)
        {
            _maxCount = maxCount;
            if (ServiceLocator.GetIfContains(out ITroopsCountView troopsCountView))
                troopsCountView.UpdateCount(_currentCount, _maxCount);
            else
                CLog.Log("ITroopsCountView not setup yet!");
        }
        
        public bool IsCellAllowed(int x, int y, ItemData item)
        {
            if (y < _minYIndex)
                return true;
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

        public void OnPutToCell(int x, int y, ItemData item)
        {
            if (y >= _minYIndex)
            {
                _currentCount++;
                if (ServiceLocator.GetIfContains(out ITroopsCountView troopsCountView))
                    troopsCountView.UpdateCount(_currentCount, _maxCount);
            }
        }
    }
}