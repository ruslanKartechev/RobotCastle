using System;

namespace RobotCastle.Merging
{
    public interface IMergeProcessor
    {
        /// <summary>
        /// Trying to merge item1 into item2
        /// </summary>
        /// <returns></returns>
        void TryMerge(IItemView item1, IItemView itemViewInto, IGridView gridView, Action<EMergeResult, bool> callback, out bool oneIntoTwo);
        
        /// <summary>
        /// Trying to merge item1 into item2
        /// </summary>
        /// <returns></returns>
        EMergeResult TryMerge(ItemData item1, ItemData item2, out ItemData mergedItem, out bool oneIntoTwo);
        Cell GetPotentialMerge(MergeGrid grid, ItemData item);
    }
}