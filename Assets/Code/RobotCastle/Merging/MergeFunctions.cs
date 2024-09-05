namespace RobotCastle.Merging
{
    public static class MergeFunctions
    {
        public static void PutItemToCell(IItemView itemView, ICellView targetCell)
        {
            itemView.Transform.position = targetCell.ItemPoint.position;
            targetCell.item = itemView;
            itemView.Data.pivotX = targetCell.cell.x;
            itemView.Data.pivotY = targetCell.cell.y;
            itemView.OnPut();
        }
        
        public static void ClearCell(IGridView gridView, IItemView item)
        {
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.currentItem = null;
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.isOccupied = false;
            gridView.Grid[item.Data.pivotX, item.Data.pivotY].item = null;
        }
        
        public static void ClearCellAndHideItem(IGridView gridView, IItemView item)
        {
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.currentItem = null;
            // gridView.Grid[item.Data.pivotX, item.Data.pivotY].cell.isOccupied = false;
            gridView.Grid[item.Data.pivotX, item.Data.pivotY].item = null;
            item.Hide();
        }
        
        public static void ClearCell(MergeGrid grid, ItemData item)
        {
            grid.rows[item.pivotY].cells[item.pivotX].currentItem = null;
            grid.rows[item.pivotY].cells[item.pivotX].isOccupied = false;
        }
    }
}