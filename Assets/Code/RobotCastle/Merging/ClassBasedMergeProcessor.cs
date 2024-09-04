using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using System;
using RobotCastle.UI;

namespace RobotCastle.Merging
{
    public class ClassBasedMergeProcessor : IMergeProcessor
    {
        /// <summary>
        /// Max level when merging items. Replace this with merging table!!
        /// </summary>
        private const int MaxItemLevel = 3;
        private const int MaxItemsCount = 3;
        private Action<EMergeResult, bool> _callback;
        
        public bool DoLog { get; set; } = true;
        
        public void TryMerge(IItemView itemViewTaken, IItemView itemViewInto, IGridView gridView,
            Action<EMergeResult, bool> callback, out bool oneIntoTwo)
        {
            _callback = callback;
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
                        _callback?.Invoke(EMergeResult.NoMerge, oneIntoTwo);
                        return;
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
                            if (TryAddItemsFromOneToAnother(itemViewTaken, itemViewInto, oneIntoTwo)) // all is OK, invoke callback
                            {
                                MergeFunctions.ClearCell(gridView, itemViewTaken);
                                itemViewTaken.Hide();
                            }
                            else // wait until the user chooses 3 items out of more > 3
                                return;
                            break;
                    }
                    _callback?.Invoke(EMergeResult.MergedIntoNew, oneIntoTwo);
                }
                // No merge, expect controller to swap them
                _callback?.Invoke(EMergeResult.NoMerge, oneIntoTwo);
                return;
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
                if (TryAddItem(viewUnit, viewItem, oneIntoTwo))
                {
                    // viewItem.Hide();
                    // MergeFunctions.ClearCell(gridView, viewItem);
                    _callback?.Invoke(EMergeResult.MergedOneIntoAnother, oneIntoTwo);
                }
            }
        }

        public EMergeResult TryMerge(ItemData item1, ItemData item2, out ItemData mergedItem, out bool oneIntoTwo)
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
                    return EMergeResult.MergedIntoNew;
                }
            }
            return EMergeResult.NoMerge;
        }

        private bool TryAddItem(IItemView viewUnit, IItemView viewItem, bool oneIntoTwo)
        {
            var itemContainerInto = viewUnit.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
            if (itemContainerInto == null)
            {
                CLog.LogRed($"NO <IUnitsItemsContainer> on {viewUnit.Transform.gameObject.name}");
                return false;
            }
            var currentItems = itemContainerInto.Items;
            var newItem = viewItem.Data.core;
            for (var i = 0; i < currentItems.Count; i++)
            {
                var item = currentItems[i];
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
                        itemContainerInto.ReplaceWithMergedItem(i, data);
                        return true;
                    }
                }
            }
            if (currentItems.Count < MaxItemsCount)
            {
                itemContainerInto.AddNewItem(newItem);
                return true;
            }
            var allItems = new List<CoreItemData>(MaxItemsCount+1);
            allItems.AddRange(currentItems);
            allItems.Add(newItem);
            CLog.LogRed($"All items (total {allItems.Count}) won't fit!!");
            var ui = ServiceLocator.Get<IUIManager>().Show<ChooseUnitItemsUI>(UIConstants.UIPickUnitItems, () => {});
            ui.PickMaximum(allItems, MaxItemsCount, (List<CoreItemData> chosen) =>
            {
                itemContainerInto.SetItems(chosen);
                _callback?.Invoke(EMergeResult.MergedOneIntoAnother, oneIntoTwo);
            });
            return false;
        }

        private bool TryAddItemsFromOneToAnother(IItemView itemTaken, IItemView itemInto, bool oneIntoTwo)
        {
            var itemContainerInto = itemInto.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
            var itemContainerTaken = itemTaken.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
            var newItemsList = MergeItemsList(itemContainerInto.Items, itemContainerTaken.Items);
            var result = false;
            if (newItemsList.Count > itemContainerInto.MaxCount)
            {
                // SHOW UI WITH SELECTION OF ITEMS
                CLog.LogRed($"All items (total {newItemsList.Count}) won't fit!!");
                var ui = ServiceLocator.Get<IUIManager>().Show<ChooseUnitItemsUI>(UIConstants.UIPickUnitItems, () => {});
                ui.PickMaximum(newItemsList, MaxItemsCount, (List<CoreItemData> chosen) =>
                {
                    itemContainerInto.SetItems(chosen);
                    _callback?.Invoke(EMergeResult.MergedOneIntoAnother, oneIntoTwo);
                } );
            }
            else
                result = true;
            itemContainerInto.SetItems(newItemsList);
            return result;
        }

        private List<CoreItemData> MergeItemsList(List<CoreItemData> items1, List<CoreItemData> items2)
        {
            var totalCount = items1.Count + items2.Count;
            if (totalCount == 0)
                return new List<CoreItemData>();

            var result = new List<CoreItemData>(totalCount);
            result.AddRange(items1);
            result.AddRange(items2);
            var db = ServiceLocator.Get<ViewDataBase>();
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
                        if (item1.level < db.GetMaxMergeLevel(item1.id))
                        {
                            item1.level++;
                            result[k] = null;
                            break;
                        }
                    }
                }                
            }

            var cleanResult = new List<CoreItemData>(result.Count); // only contains non-null values
            for (var i = result.Count - 1; i >= 0; i--)
            {
                if (result[i] != null)
                    cleanResult.Add(result[i]);
            }
            return cleanResult;
        }

        public Cell GetPotentialMerge(MergeGrid grid, ItemData item)
        {
            return null;
        }



        public class ChooseItemsOffer
        {
            public ChooseItemsOffer(IItemView itemViewTaken, 
                IItemView itemViewInto, 
                Action<EMergeResult> callback,
                List<CoreItemData> items)
            {
                this.itemViewTaken = itemViewTaken;
                this.itemViewInto = itemViewInto;
                _callback = callback;
                this.items = items;
            }

            private IItemView itemViewTaken;
            private IItemView itemViewInto;
            private List<CoreItemData> items;
            private Action<EMergeResult> _callback;
            

            public void Show()
            {
                var ui = ServiceLocator.Get<IUIManager>().Show<ChooseUnitItemsUI>(UIConstants.UIPickUnitItems, () => {});
                ui.PickMaximum(items, 3, OnChosen);
            }

            private void OnChosen(List<CoreItemData> pickedItems)
            {
                                
            }
        } 
    }
}