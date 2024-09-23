using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IGridSectionsController
    {
        void SetGridView(IGridView gridView);
        bool IsCellAllowed(int x, int y, ItemData item, bool promptUser = true);
        void SetMaxCount(int maxCount);
        int GetMaxCount();
        /// <summary>
        /// Internally calls OnGridUpdated()
        /// </summary>
        void OnItemPut(ItemData item);
        bool GetFreeCell(MergeGrid grid, out Vector2Int coordinates);
        
        int GetFreeCellsCount();
        List<ItemData> GetAllItems();
        List<ItemData> GetAllItemsInMergeArea();
        List<IItemView> GetAllItemViewsInMergeArea();
        List<T> GetItemsInActiveArea<T>(MiscUtils.Condition<T> condition);

        bool CanPutMoreIntoActiveZone();
        Vector2Int GetCoordinateForClosestCellInActiveZone(Vector2Int originalCell);


    }
}