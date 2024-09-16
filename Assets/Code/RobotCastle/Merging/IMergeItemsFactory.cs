namespace RobotCastle.Merging
{
    public interface IMergeItemsFactory
    {
        void SpawnItemsForGrid(IGridView gridView, MergeGrid saves);
        IItemView SpawnItemOnCell(ICellView pivotCell, ItemData item);
        IItemView SpawnItemOnCell(ICellView pivotCell, string itemId);
        
        
    }
}