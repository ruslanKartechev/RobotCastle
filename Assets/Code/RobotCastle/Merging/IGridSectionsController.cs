using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IGridSectionsController
    {
        void SetGridView(IGridView gridView);
        bool IsCellAllowed(int x, int y, ItemData item, bool promptUser = true);
        void OnGridUpdated();
        void SetMaxCount(int maxCount);
        
        /// <summary>
        /// Internally calls OnGridUpdated()
        /// </summary>
        void OnItemPut(ItemData item);
        
        bool GetFreeCell(MergeGrid grid, out Vector2Int coordinates);
        bool IsCellFree(MergeGrid grid, ItemData itemData, Vector2Int coordinated);        
        
        int GetFreeCellsCount(MergeGrid grid);
        List<ItemData> GetAllItems();
        List<ItemData> GetAllItemsInMergeArea();
        List<IItemView> GetAllItemViewsInMergeArea();
        List<T> GetItemsInActiveArea<T>(MiscUtils.Condition<T> conditionDelegate);


    }
}