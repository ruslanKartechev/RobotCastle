namespace RobotCastle.Merging
{
    public interface IGridItemsSpawner
    {
        void SpawnItemsForGrid(IGridView gridView, MergeGrid saves);
        IItemView SpawnItemOnCell(ICellView cell, ItemData item);
        IItemView SpawnItemOnCell(ICellView cell, string itemId);
        
        
    }
}