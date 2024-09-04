using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class CellAvailabilityControllerBySection : MonoBehaviour, ICellAvailabilityController
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