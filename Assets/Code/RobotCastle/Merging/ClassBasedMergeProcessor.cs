    using System.Collections.Generic;
    using RobotCastle.Battling;
    using RobotCastle.Core;
    using SleepDev;
    using System;
    using RobotCastle.UI;
    using UnityEngine;

    namespace RobotCastle.Merging
    {
        public class ClassBasedMergeProcessor : IMergeProcessor, IItemsChoiceListener
        {
            /// <summary>
            /// Max level when merging items. Replace this with merging table!!
            /// </summary>
            private const int MaxItemLevel = 3;
            private const int MaxItemsCount = 3;
            private Action<EMergeResult, bool> _callback;
            private IItemView _itemViewTaken;
            private IItemView _itemViewInto;
            private IGridView _gridView;
            private bool _oneIntoTwo;
            
            public bool DoLog { get; set; } = true;
            
            public void TryMerge(IItemView itemViewTaken, IItemView itemViewInto, IGridView gridView,
                Action<EMergeResult, bool> callback, out bool oneIntoTwo)
            {
                _itemViewTaken = itemViewTaken;
                _itemViewInto = itemViewInto;
                _callback = callback;
                _gridView = gridView;
                var itemTaken = itemViewTaken.Data;
                var itemInto = itemViewInto.Data;
                // ItemData mergedItem = null;
                oneIntoTwo = true;
                if(DoLog)
                    CLog.Log($"Trying to merge\n1: {itemTaken.GetStr()}\n2: {itemInto.GetStr()}\n Pos_1 {{{itemTaken.pivotX},{itemTaken.pivotY}}}\n Pos_2 {{{itemInto.pivotX},{itemInto.pivotY}}}");
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
                                var mergedItem = new ItemData(itemInto);
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
                                // mergedItem = new ItemData(itemInto);
                                // mergedItem.core.level++;
                                itemViewInto.Data.core.level++;
                                itemViewInto.UpdateViewToData(itemViewInto.Data);
                                if (TryAddItemsFromOneToAnother(itemViewTaken, itemViewInto, oneIntoTwo)) // all is OK, invoke callback
                                {
                                    MergeFunctions.ClearCellAndHideItem(gridView, itemViewTaken);
                                }
                                else // wait until the user chooses 3 items out of more > 3
                                    return;
                                break;
                        }
                        _callback?.Invoke(EMergeResult.MergedIntoNew, oneIntoTwo);
                    }
                    else
                    {
                        // No merge, expect controller to swap them
                        _callback?.Invoke(EMergeResult.NoMerge, oneIntoTwo);                        
                    }
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
                    _oneIntoTwo = oneIntoTwo;
                    if (TryAddItem(viewUnit, viewItem, oneIntoTwo))
                    {
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
                            MergeFunctions.ClearCellAndHideItem(_gridView, viewItem);
                            return true;
                        }
                    }
                }
                if (currentItems.Count < MaxItemsCount)
                {
                    itemContainerInto.AddNewItem(newItem);
                    if (oneIntoTwo) // dragged into standing. Item into Unit
                    {
                        MergeFunctions.ClearCell(_gridView, viewItem);
                        viewItem.Hide();
                    }
                    else // standing into dragged. Unit into item
                    {
                        MergeFunctions.ClearCell(_gridView, viewUnit);
                        var targetCell = _gridView.GetCell(viewItem.Data.pivotX, viewItem.Data.pivotY);
                        MergeFunctions.ClearCellAndHideItem(_gridView, viewItem);
                        MergeFunctions.PutItemToCell(viewUnit, targetCell);
                    }
                    return true;
                }
                var allItems = new List<CoreItemData>(MaxItemsCount * 2);
                allItems.AddRange(currentItems);
                allItems.Add(newItem);
                OfferChooseItems(allItems);
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
                    OfferChooseItems(newItemsList);
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

            private void OfferChooseItems(List<CoreItemData> items)
            {
                CLog.LogRed($"All items (total {items.Count}) won't fit!!");
                var ui = ServiceLocator.Get<IUIManager>().Show<ChooseUnitItemsUI>(UIConstants.UIPickUnitItems, () => {});
                ui.PickMaximum(items, MaxItemsCount, this);
            }

            public void ConfirmChosenItems(List<CoreItemData> chosen, List<CoreItemData> left)
            {
                foreach (var it in chosen)
                    CLog.LogGreen($"Chosen: {it.ItemDataStr()}");
                foreach (var it in left)
                    CLog.LogBlue($"Left: {it.ItemDataStr()}");
                
                if (_oneIntoTwo) // dragged into standing. Unit is standing
                {
                    var container = _itemViewInto.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
                    container.SetItems(chosen);
                    MergeFunctions.ClearCellAndHideItem(_gridView, _itemViewTaken);
                }
                else // standing into dragged. Unit is dragged
                {
                    var container = _itemViewTaken.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
                    container.SetItems(chosen);
                    
                    MergeFunctions.ClearCell(_gridView, _itemViewTaken);
                    var targetCell = _gridView.GetCell(_itemViewInto.Data.pivotX, _itemViewInto.Data.pivotY);
                    MergeFunctions.ClearCellAndHideItem(_gridView, _itemViewInto);
                    MergeFunctions.PutItemToCell(_itemViewTaken, targetCell);
                }

                var spawner = ServiceLocator.Get<IGridItemsSpawner>();
                var cellPicker = ServiceLocator.Get<ICellAvailabilityController>();
                foreach (var coreData in left)
                {
                    var hasCell = cellPicker.GetFreeCell(_gridView.BuiltGrid, out var coords);
                    if (!hasCell)
                    {
                        CLog.LogRed($"[{nameof(ClassBasedMergeProcessor)}] No more free cells to put items");
                        break;
                    }
                    var cell = _gridView.GetCell(coords.x, coords.y);
                    spawner.SpawnItemOnCell(cell, new ItemData(coreData));
                }
                _callback?.Invoke(EMergeResult.MergedOneIntoAnother, _oneIntoTwo);
            }
            
            public List<Vector2Int> GetCellsForPotentialMerge(MergeGrid grid, ItemData srcItem)
            {
                if (srcItem.core.type != MergeConstants.TypeUnits)
                    return null;
                var db = ServiceLocator.Get<ViewDataBase>();
                var maxLvl = db.GetMaxMergeLevel(srcItem.core.id);
                if (srcItem.core.level >= maxLvl)
                    return null;
                var list = new List<Vector2Int>(3);
                for (var y = 0; y < grid.rows.Count; y++)
                {
                    var row = grid.rows[y].cells;
                    for (var x = 0; x < row.Count; x++)
                    {
                        var cell = row[x];
                        if (!cell.isOccupied)
                            continue;
                        if (cell.currentItem == srcItem)
                            continue;
                        if (cell.currentItem.core == srcItem.core)
                        {
                            list.Add(new Vector2Int(cell.x, cell.y));   
                        }
                    }   
                }
                return list;
            }

            public List<Vector2Int> GetCellsForPotentialMerge(List<ItemData> allItems, ItemData srcItem)
            {
                if (srcItem.core.type != MergeConstants.TypeUnits)
                    return null;
                var db = ServiceLocator.Get<ViewDataBase>();
                var maxLvl = db.GetMaxMergeLevel(srcItem.core.id);
                if (srcItem.core.level >= maxLvl)
                    return null;
                var list = new List<Vector2Int>(3);
                foreach (var otherItem in allItems)
                {
                    if(otherItem == srcItem)
                        continue;
                    if(otherItem.core == srcItem.core)
                        list.Add(new Vector2Int(otherItem.pivotX, otherItem.pivotY));
                }
                return list;
            }
        } 
    }