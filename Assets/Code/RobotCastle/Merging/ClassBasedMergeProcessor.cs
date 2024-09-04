using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;

namespace RobotCastle.Merging
{
    public class ClassBasedMergeProcessor : IMergeProcessor
    {
        /// <summary>
        /// Max level when merging items. Replace this with merging table!!
        /// </summary>
        private const int MaxItemLevel = 3;

        public bool DoLog { get; set; } = true;
        
        public MergeResult TryMerge(IItemView itemViewTaken, IItemView itemViewInto, IGridView gridView, out bool oneIntoTwo)
        {
            var itemTaken = itemViewTaken.Data;
            var itemInto = itemViewInto.Data;
            ItemData mergedItem = null;
            oneIntoTwo = true;
            if(DoLog)
                CLog.Log($"Trying to merge. 1: {itemTaken.GetStr()}. 2: {itemInto.GetStr()} || Pos_1 {{{itemTaken.pivotX},{itemTaken.pivotY}}} || Pos_2 {{{itemInto.pivotX},{itemInto.pivotY}}}");
            if (itemTaken.core.type == itemInto.core.type) // Type is the Same
            {
                if (itemTaken.core.id == itemInto.core.id
                    && itemTaken.core.level == itemInto.core.level) // Could be merged
                {
                    var db = ServiceLocator.Get<ViewDataBase>();
                    if (itemInto.core.level >= db.GetMaxMergeLevel(itemInto.core.id))
                    {
                        CLog.Log($"{itemInto.core.level} is already max merge level");
                        return MergeResult.NoMerge;
                    }
                    switch (itemInto.core.type)
                    {
                        case MergeConstants.TypeItems:
                            mergedItem = new ItemData(itemInto);
                            mergedItem.core.level++;
                            itemViewInto.Hide();
                            itemViewTaken.Hide();
                            MergeFunctions.ClearCell(gridView, itemViewTaken);
                            MergeFunctions.ClearCell(gridView, itemViewInto);
                        
                            var spawner = ServiceLocator.Get<IGridItemsSpawner>();
                            var cell = gridView.Grid[itemInto.pivotX, itemInto.pivotY];
                            spawner.SpawnItemOnCell(cell, mergedItem);
                        
                            break;
                        case MergeConstants.TypeUnits:
                            mergedItem = new ItemData(itemInto);
                            mergedItem.core.level++;
                            itemViewInto.UpdateViewToData(mergedItem);
                            TryAddItemsFromOneToAnother(itemViewTaken, itemViewInto);
                        
                            MergeFunctions.ClearCell(gridView, itemViewTaken);
                            itemViewTaken.Hide();
                            break;
                    }
                    return MergeResult.MergedIntoNew;
                }
                // No merge, expect controller to swap them
                return MergeResult.NoMerge;
            }
            else // Two different types
            {
                IItemView viewItem = null;
                IItemView viewUnit = null;
                if (itemTaken.core.type == MergeConstants.TypeUnits && itemInto.core.type == MergeConstants.TypeItems)
                {
                    viewUnit = itemViewTaken;
                    viewItem = itemViewInto;
                    oneIntoTwo = false;
                }
                else if (itemTaken.core.type == MergeConstants.TypeItems && itemInto.core.type == MergeConstants.TypeUnits)
                {
                    viewUnit = itemViewInto;
                    viewItem = itemViewTaken;
                    oneIntoTwo = true;
                }
                if (TryAddItem(viewUnit, viewItem))
                {
                    viewItem.Hide();
                    MergeFunctions.ClearCell(gridView, viewItem);
                    return MergeResult.MergedOneIntoAnother;
                }
                return MergeResult.NoMerge;
            }
            return MergeResult.NoMerge;
        }

        public MergeResult TryMerge(ItemData item1, ItemData item2, out ItemData mergedItem, out bool oneIntoTwo)
        {
            oneIntoTwo = true;
            mergedItem = null;
            // CLog.Log($"Trying to merge. 1: {item1.GetStr()}. 2: {item2.GetStr()}");
            if (item1.core.type == item2.core.type)
            {
                if (item1.core.id == item2.core.id && item1.core.level == item2.core.level)
                {
                    mergedItem = new ItemData(item1);
                    mergedItem.core.level++;
                    return MergeResult.MergedIntoNew;
                }
            }
            return MergeResult.NoMerge;
        }

        private bool TryAddItem(IItemView viewUnit, IItemView viewItem)
        {
            var container = viewUnit.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
            if (container == null)
            {
                CLog.LogRed($"NO <IUnitsItemsContainer> on {viewUnit.Transform.gameObject.name}");
                return false;
            }
            var items = container.Items;
            var newItem = viewItem.Data.core;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.id == newItem.id)
                {
                    if (item.level == newItem.level && item.level < MaxItemLevel)
                    {
                        var data = new CoreItemData()
                        {
                            id = newItem.id,
                            level = newItem.level + 1,
                            type = newItem.type
                        };
                        container.ReplaceWithMergedItem(i, data);
                        return true;
                    }
                }
            }
            if (items.Count < container.MaxCount)
            {
                container.AddNewItem(newItem);
                return true;
            }
            return false;
        }

        private bool TryAddItemsFromOneToAnother(IItemView itemTaken, IItemView itemInto)
        {
            var itemContainerInto = itemInto.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
            var itemContainerTaken = itemTaken.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
            var newItemsList = MergeItemsList(itemContainerInto.Items, itemContainerTaken.Items);
            if (newItemsList.Count > itemContainerInto.MaxCount)
            {
                // SHOW UI WITH SELECTION OF ITEMS
                CLog.LogRed($"All items (total {newItemsList.Count}) won't fit!!");
            }
            itemContainerInto.SetItems(newItemsList);
            return true;
        }

        private List<CoreItemData> MergeItemsList(List<CoreItemData> items1, List<CoreItemData> items2)
        {
            var totalCount = items1.Count + items2.Count;
            if (totalCount == 0)
                return new List<CoreItemData>();

            var result = new List<CoreItemData>(totalCount);
            result.AddRange(items1);
            result.AddRange(items2);

            for (var i = 0; i < result.Count; i++)
            {
                if(result[i] is null)
                    continue;
                var item1 = result[i];
                for (var k = i + 1; k < result.Count; k++)
                {
                    var item2 = result[k];
                    if (item1 == item2)
                    {
                        item1.level++;
                        result[k] = null;
                        break;
                    }
                }                
            }
            return result;
        }

        public Cell GetPotentialMerge(MergeGrid grid, ItemData item)
        {
            return null;
        }
    }
}