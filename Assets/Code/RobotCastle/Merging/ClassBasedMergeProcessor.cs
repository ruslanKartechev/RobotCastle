using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using System;
using RobotCastle.Data;
using RobotCastle.Utils;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class ClassBasedMergeProcessor : IMergeProcessor
    {
        public bool DoLog { get; set; } = true;
        
        private const int MaxItemsCount = 3;
        private Action<EMergeResult, bool> _callback;
        private IItemView _itemViewTaken;
        private IItemView _itemViewInto;
        private IGridView _gridView;
        private IMergeItemsContainer _container;
        private IMergeMaxLevelCheck _maxLevelCheck;
            
        private readonly List<IMergeModifier> _modifiers = new (10);

        public ClassBasedMergeProcessor(IMergeItemsContainer itemsContainer, IMergeMaxLevelCheck maxLevelCheck)
        {
            _container = itemsContainer;
            _maxLevelCheck = maxLevelCheck;
        }
        
        public void AddModifier(IMergeModifier mod)
        {
            if (_modifiers.Contains(mod)) return;
            _modifiers.Add(mod);
        }

        public void RemoveModifier(IMergeModifier mod)
        {
            _modifiers.Remove(mod);
        }

        public void ClearAllModifiers() => _modifiers.Clear();
        
        public void BreakItemToReturn(IItemView item)
        {
            var data = item.itemData;
            var go = item.Transform.gameObject;
            item.Hide();
            if (data.core.type == MergeConstants.TypeHeroes)
            {
                var weapons = go.GetComponent<IHeroWeaponsContainer>();
                if (weapons.Items.Count > 0)
                {
                    var factory = ServiceLocator.Get<IPlayerMergeItemsFactory>();
                    foreach (var weaponData in weapons.Items)
                    {
                        var itemData = new ItemData(weaponData.core);
                        var args = new SpawnMergeItemArgs(itemData.core);
                        var spawned = factory.SpawnHeroOrItem(args);
                        if (spawned == null)
                            return;
                        var container = spawned.Transform.gameObject.GetComponent<ModifiersContainer>();
                        if(container != null)
                            container.OverrideModifiers(weaponData.modifierIds);
                        else
                            CLog.Log($"[BreakItemToReturn] Couldn't find ModifiersContainer to override modifiers");
                    }
                }
            }
        }

        public void TryMerge(IItemView itemViewTaken, IItemView itemViewInto, IGridView gridView, Action<EMergeResult, bool> callback)
        {
            if (itemViewTaken.itemData.core.id == MergeConstants.UpgradeBookId)
            {
                var upgradeOperation = new UpgradeWithBookOperation(itemViewTaken, itemViewInto, true, gridView, _container, _maxLevelCheck, callback);
                upgradeOperation.Process();
                return;
            }
            if (itemViewInto.itemData.core.id == MergeConstants.UpgradeBookId)
            {
                var upgradeOperation = new UpgradeWithBookOperation(itemViewInto, itemViewTaken, false, gridView, _container, _maxLevelCheck, callback);
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
            if(DoLog)
                CLog.Log($"Trying to merge\n1: {itemTaken.GetStr()}\n2: {itemInto.GetStr()}\n Pos_1 {{{itemTaken.pivotX},{itemTaken.pivotY}}}\n Pos_2 {{{itemInto.pivotX},{itemInto.pivotY}}}");
              
            var differentTypes = itemTaken.core.type != itemInto.core.type;
            if (differentTypes)
            {
                var operation3 = new AddItemToUnitOperation(_itemViewTaken, _itemViewInto, _gridView, _container, _callback, _modifiers);
                operation3.Process();
                return;
            }
                
            var couldBeMerged = itemTaken.core.id == itemInto.core.id && itemTaken.core.level == itemInto.core.level;
            if (!couldBeMerged)
            {
                // No merge, expect controller to swap them
                _callback?.Invoke(EMergeResult.NoMerge, true);                
                return;
            }
            if (_maxLevelCheck.CanUpgradeFurther(itemInto.core) == false)
            {
                _callback?.Invoke(EMergeResult.NoMerge, true);
                return;
            }
            switch (itemInto.core.type)
            {
                case MergeConstants.TypeWeapons:
                    var operation1 = new MergeWeaponsOperation(itemViewTaken, itemViewInto, gridView, _container, _maxLevelCheck, _callback, _modifiers);
                    operation1.Process();
                    return;
                case MergeConstants.TypeHeroes:
                    var operation2 = new MergeUnitsOperation(itemViewTaken, itemViewInto, _gridView, _container, _maxLevelCheck, _callback, _modifiers);
                    operation2.Process();
                    return;
            }
            _callback?.Invoke(EMergeResult.MergedIntoNew, true);
        }

            
        public List<Vector2Int> GetCellsForPotentialMerge(List<ItemData> allItems, ItemData srcItem)
        {
            var db = ServiceLocator.Get<ViewDataBase>();
            var maxLvl = db.GetMaxMergeLevel(srcItem.core.id) - 1;
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
            allItems.ClearNulls();
            return allItems;
            
            // Nested Methods
            bool TryMakeOneMerge()
            {
                var count = allItems.Count;
                for (var indOne = count - 1; indOne >= 1; indOne--)
                {
                    if (allItems[indOne] == null)
                        continue;
                    var data1 = allItems[indOne].itemData.core;
                    var maxLvl = db.GetMaxMergeLevel(data1.id) - 1; // (not indexed)
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
                                case MergeConstants.TypeHeroes:
                                    var itemsCont1 = allItems[indOne].Transform.gameObject.GetComponent<IHeroWeaponsContainer>();
                                    var itemsCont2 = allItems[indTwo].Transform.gameObject.GetComponent<IHeroWeaponsContainer>();
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
                                    _container.RemoveItem(allItems[indTwo]);
                                    MergeFunctions.ClearCellAndHideItem(gridView, allItems[indTwo]);
                                    allItems[indTwo] = null;
                                    data1.level++;
                                    allItems[indOne].UpdateViewToData();
                                    allItems[indOne].OnMerged();
                                    return true;
                                case MergeConstants.TypeWeapons:
                                    _container.RemoveItem(allItems[indOne]);
                                    _container.RemoveItem(allItems[indTwo]);
                                    MergeFunctions.ClearCellAndHideItem(gridView, allItems[indOne]);
                                    MergeFunctions.ClearCellAndHideItem(gridView, allItems[indTwo]);
                                    allItems[indOne] = null;
                                    allItems[indTwo] = null;
                                    data1.level++;
                                    allItems[indOne] = ServiceLocator.Get<IMergeItemsFactory>().SpawnItemOnCell(cellOne, new ItemData(data1));
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
                if (core1.type == MergeConstants.TypeHeroes || core2.type == MergeConstants.TypeWeapons)
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



        private List<HeroWeaponData> MergeItemsList(List<HeroWeaponData> items1, List<HeroWeaponData> items2)
        {
            var totalCount = items1.Count + items2.Count;
            if (totalCount == 0)
                return new List<HeroWeaponData>();

            var result = new List<HeroWeaponData>(totalCount);
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
                        if (_maxLevelCheck.CanUpgradeFurther(item1.core))
                        {
                            item1.core.level++;
                            item1.modifierIds.AddRange(item2.modifierIds);
                            result[k] = null;
                            break;
                        }
                    }
                }                
            }
            
            var cleanResult = new List<HeroWeaponData>(result.Count); // only contains non-null values
            for (var i = result.Count - 1; i >= 0; i--)
            {
                if (result[i] != null)
                    cleanResult.Add(result[i]);
            }
            return cleanResult;
        }
        
    }
    
}