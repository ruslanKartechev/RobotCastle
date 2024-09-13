    using System.Collections.Generic;
    using RobotCastle.Battling;
    using RobotCastle.Core;
    using SleepDev;
    using System;
    using UnityEngine;

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
            private IItemView _itemViewTaken;
            private IItemView _itemViewInto;
            private IGridView _gridView;
            
            public bool DoLog { get; set; } = true;
            
            public void TryMerge(IItemView itemViewTaken, IItemView itemViewInto, IGridView gridView,
                Action<EMergeResult, bool> callback, out bool oneIntoTwo)
            {
                oneIntoTwo = true;
                if (itemViewTaken.itemData.core.id == MergeConstants.UpgradeBookId)
                {
                    var upgradeOperation = new UpgradeWithBookOperation(itemViewTaken, itemViewInto, true, gridView, callback);
                    upgradeOperation.Process();
                    return;
                }
                if (itemViewInto.itemData.core.id == MergeConstants.UpgradeBookId)
                {
                    var upgradeOperation = new UpgradeWithBookOperation(itemViewInto, itemViewTaken, false, gridView, callback);
                    upgradeOperation.Process();
                    return;
                }
                
                _itemViewTaken = itemViewTaken;
                _itemViewInto = itemViewInto;
                _callback = callback;
                _gridView = gridView;
                var itemTaken = itemViewTaken.itemData;
                var itemInto = itemViewInto.itemData;
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
                                itemViewInto.itemData.core.level++;
                                itemViewInto.UpdateViewToData();
                                var process = new MergeUnitsWithItemsOperation(itemViewTaken, itemViewInto, _gridView, _callback);
                                process.Process();
                                return;
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
                    if (itemTaken.core.type == MergeConstants.TypeUnits && itemInto.core.type == MergeConstants.TypeItems)
                        oneIntoTwo = false;
                    else if (itemTaken.core.type == MergeConstants.TypeItems && itemInto.core.type == MergeConstants.TypeUnits)
                        oneIntoTwo = true;
                    var operation = new AddItemToUnitOperation(_itemViewTaken, _itemViewInto, _gridView, _callback);
                    operation.Process();
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

            private bool TryAddItemsFromOneToAnother(IItemView itemTaken, IItemView itemInto)
            {
                var itemContainerInto = itemInto.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
                var itemContainerTaken = itemTaken.Transform.gameObject.GetComponent<IUnitsItemsContainer>();
                var newItemsList = MergeItemsList(itemContainerInto.Items, itemContainerTaken.Items);
                var result = false;
                if (newItemsList.Count > itemContainerInto.MaxCount)
                {
                    // OfferChooseItems(newItemsList);
                }
                else
                    result = true;
                itemContainerInto.SetItems(newItemsList);
                return result;
            }

            public List<Vector2Int> GetCellsForPotentialMerge(MergeGrid grid, ItemData srcItem)
            {
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

            public List<IItemView> MergeAllItemsPossible(List<IItemView> allItems, IGridView gridView)
            {
                if (allItems.Count < 2)
                    return allItems;
                var db = ServiceLocator.Get<ViewDataBase>();
                var it = 0;
                const int itMax = 100;
                while (it < itMax && TryMakeOneMerge())
                    it++;
                if(it >= itMax)
                    CLog.LogRed("[MergeAllItemsPossible] it >= it_max !!! ERROR");
                var result = new List<IItemView>(allItems.Count);
                for (var i = 0; i < allItems.Count; i++)
                {
                    if(allItems[i] != null)
                        result.Add(allItems[i]);
                }
                return result;

                // Nested Methods
                bool TryMakeOneMerge()
                {
                    var count = allItems.Count;
                    for (var indOne = count - 1; indOne >= 1; indOne--)
                    {
                        if (allItems[indOne] == null)
                            continue;
                        var data1 = allItems[indOne].itemData.core;
                        var maxLvl = db.GetMaxMergeLevel(data1.id);
                        if (data1.level >= maxLvl)
                            continue;
                        var cellOne = gridView.GetCell(allItems[indOne].itemData.pivotX, allItems[indOne].itemData.pivotY);
                        for (var indTwo = indOne - 1; indTwo >= 0; indTwo--)
                        {
                            if (allItems[indTwo] == null)
                                continue;
                            var data2 = allItems[indTwo].itemData.core;
                            if (data1 == data2)
                            {
                                switch (data1.type)
                                {
                                    case MergeConstants.TypeUnits:
                                        var itemsCont1 = allItems[indOne].Transform.gameObject.GetComponent<IUnitsItemsContainer>();
                                        var itemsCont2 = allItems[indTwo].Transform.gameObject.GetComponent<IUnitsItemsContainer>();
                                        if (itemsCont1.ItemsCount + itemsCont2.ItemsCount > 0)
                                        {
                                            var merged = MergeItemsList(itemsCont1.Items, itemsCont2.Items);
                                            if (merged.Count <= MaxItemsCount)
                                                itemsCont1.UpdateItems(merged);
                                            else
                                            {
                                                CLog.Log($"[MergeAll] All items won't fit into container");
                                                continue;
                                            }
                                        }
                                        MergeFunctions.ClearCellAndHideItem(gridView, allItems[indTwo]);
                                        allItems[indTwo] = null;
                                        data1.level++;
                                        allItems[indOne].UpdateViewToData();
                                        return true;
                                    case MergeConstants.TypeItems:
                                        MergeFunctions.ClearCellAndHideItem(gridView, allItems[indOne]);
                                        MergeFunctions.ClearCellAndHideItem(gridView, allItems[indTwo]);
                                        allItems[indOne] = null;
                                        allItems[indTwo] = null;
                                        data1.level++;
                                        allItems[indOne] = ServiceLocator.Get<IGridItemsSpawner>().SpawnItemOnCell(cellOne, new ItemData(data1));
                                        return true;
                                }
                            }
                        }
                    }
                    return false;
                }
                
            }

            public List<IItemView> SortAllItemsPossible(List<IItemView> allItems, IGridView gridView)
            {
                if (allItems.Count < 2)
                    return allItems;
                var result = new List<IItemView>(allItems);
                result.Sort((a, b) =>
                {
                    var core1 = a.itemData.core;
                    var core2 = b.itemData.core;
                    if (core1.type == core2.type)
                    {
                        if (core1.id == core2.id)
                            return core2.level.CompareTo(core1.level);
                        else
                            return String.Compare(core2.id, core1.id, StringComparison.Ordinal);
                    }
                    if (core1.type == MergeConstants.TypeUnits || core2.type == MergeConstants.TypeItems)
                        return -1;
                    else
                        return 1;
                });
                var maxY = gridView.Grid.GetLength(1);
                var maxX = gridView.Grid.GetLength(0);
                foreach (var item in result)
                    MergeFunctions.ClearCell(gridView, item);
                var itemInd = 0;
                for (var y = 0; y < maxY; y++)
                {
                    for (var x = 0; x < maxX; x++)
                    {
                        var cell = gridView.GetCell(x, y);
                        if (cell.cell.isOccupied == false)
                        {
                            MergeFunctions.PutItemToCell(result[itemInd], cell);
                            itemInd++;
                            if (itemInd == result.Count)
                            {
                                return result;
                            }
                        }
                    }                    
                }
                return result;
            }
            
            public List<CoreItemData> MergeItemsList(List<CoreItemData> items1, List<CoreItemData> items2)
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
        }
    }