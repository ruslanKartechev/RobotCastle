using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IGridSectionsController
    {
        bool IsCellAllowed(int x, int y, ItemData item, bool promptUser = true);
        void OnGridUpdated(MergeGrid grid);
        void OnItemPut(ItemData item);
        bool GetFreeCell(MergeGrid grid, out Vector2Int coordinates);
        int GetFreeCellsCount(MergeGrid grid);
        List<ItemData> GetAllItems();
        List<ItemData> GetAllItemsInMergeArea();
        List<IItemView> GetAllItemViewsInMergeArea();


    }
}