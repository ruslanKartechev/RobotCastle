namespace RobotCastle.Merging
{
    public interface IMergeProcessor
    {
        /// <summary>
        /// Trying to merge item1 into item2
        /// </summary>
        /// <returns></returns>
        MergeResult TryMerge(IItemView item1, IItemView itemViewInto, IGridView gridView, out bool oneIntoTwo);
        
        /// <summary>
        /// Trying to merge item1 into item2
        /// </summary>
        /// <returns></returns>
        MergeResult TryMerge(ItemData item1, ItemData item2, out ItemData mergedItem, out bool oneIntoTwo);
        Cell GetPotentialMerge(MergeGrid grid, ItemData item);
    }
}