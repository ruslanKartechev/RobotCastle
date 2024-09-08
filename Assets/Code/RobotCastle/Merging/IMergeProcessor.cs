using System;
using System.Collections.Generic;
using UnityEngine;

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
        
        List<Vector2Int> GetCellsForPotentialMerge(List<ItemData> allItems, ItemData srcItem);

        List<IItemView> MergeAllItemsPossible(List<IItemView> allItems, IGridView gridView);
        List<IItemView> SortAllItemsPossible(List<IItemView> allItems, IGridView gridView);
        
    }
}