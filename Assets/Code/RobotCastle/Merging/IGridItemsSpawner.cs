namespace RobotCastle.Merging
{
    public interface IGridItemsSpawner
    {
        void SpawnItemsForGrid(IGridView gridView, MergeGrid saves);
        void SpawnItemOnCell(ICellView cell, ItemData item);
        void SpawnItemOnCell(ICellView cell, string itemId);
        
        
    }
}