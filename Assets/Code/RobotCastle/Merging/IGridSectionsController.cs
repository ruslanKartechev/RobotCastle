using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IGridSectionsController
    {
        void SetGridView(IGridView gridView);
        bool IsCellAllowed(Vector2Int coord, ItemData item, bool promptUser = true);
        void SetMaxCount(int maxCount);
        /// <summary>
        /// Internally calls OnGridUpdated()
        /// </summary>
        void OnItemPut(ItemData item);
        void OnGridUpdated();
        bool GetFreeAllowedCell(MergeGrid grid, ItemData itemData, out Vector2Int coordinates);
        
        List<ItemData> GetAllItems();
        List<IItemView> GetAllItemsViews();
        
        List<ItemData> GetAllItemsInMergeArea();
        List<IItemView> GetAllItemViewsInMergeArea();
        List<T> GetItemsInActiveArea<T>(MiscUtils.Condition<T> condition);

        bool CanPutMoreIntoActiveZone();
        Vector2Int GetCoordinateForClosestCellInActiveZone(Vector2Int originalCell);


    }
}