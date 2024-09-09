using System.Collections.Generic;

namespace RobotCastle.Merging
{
    public static class MergeFunctions
    {
        public static void PutItemToCell(IItemView itemView, ICellView targetCell)
        {
            itemView.Transform.position = targetCell.ItemPoint.position;
            itemView.Transform.rotation = targetCell.ItemPoint.rotation;
            
            targetCell.itemView = itemView;
            itemView.itemData.pivotX = targetCell.cell.x;
            itemView.itemData.pivotY = targetCell.cell.y;
            itemView.OnPut();
        }
        
        public static void ClearCell(IGridView gridView, IItemView item)
        {
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.currentItem = null;
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.isOccupied = false;
            gridView.Grid[item.itemData.pivotX, item.itemData.pivotY].itemView = null;
        }
        
        public static void ClearCellAndHideItem(IGridView gridView, IItemView item)
        {
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.currentItem = null;
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.isOccupied = false;
            gridView.Grid[item.itemData.pivotX, item.itemData.pivotY].itemView = null;
            item.Hide();
        }
        
        public static void ClearCell(MergeGrid grid, ItemData item)
        {
            grid.rows[item.pivotY].cells[item.pivotX].currentItem = null;
            grid.rows[item.pivotY].cells[item.pivotX].isOccupied = false;
        }

        public static List<CoreItemData> TryMergeAll(List<CoreItemData> items, int maxLvl)
        {
            var result = new List<CoreItemData>(items);
            var can = true;
            while (can)
                can = TryMergeInList(result, maxLvl);
            return result;
        }

        public static bool TryMergeInList(List<CoreItemData> items, int maxLvl)
        {
            for (var i = items.Count - 1; i >= 1; i--)
            {
                var item = items[i];
                if (item.level >= maxLvl)
                    continue;
                for (var k = i - 1; k >= 0; k--)
                {
                    if (item == items[k])
                    {
                        item.level++;
                        items.RemoveAt(k);
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}