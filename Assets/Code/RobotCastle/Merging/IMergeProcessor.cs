using System;
using System.Collections.Generic;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Merging
{
    public interface IMergeProcessor : IModifiable<IMergeModifier>
    {
        /// <summary>
        /// Trying to merge item1 into item2
        /// </summary>
        /// <returns></returns>
        void TryMerge(IItemView item1, IItemView itemViewInto, 
            IGridView gridView, Action<EMergeResult, bool> callback);

        void BreakItemToReturn(IItemView item);
        
        List<Vector2Int> GetCellsForPotentialMerge(List<ItemData> allItems, ItemData srcItem);

        List<IItemView> MergeAllItemsPossible(List<IItemView> allItems, IGridView gridView);
        List<IItemView> SortAllItemsPossible(List<IItemView> allItems, IGridView gridView);

    }
}