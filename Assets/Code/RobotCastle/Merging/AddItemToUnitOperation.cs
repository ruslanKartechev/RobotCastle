using System;
using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Merging
{
    public class AddItemToUnitOperation : IItemsChoiceListener
    {
        
        private Action<EMergeResult, bool> _callback;
        private bool _oneIntoTwo;
        private IItemView _unitView;
        private IItemView _itemView;
        private IGridView _gridView;
        private ChooseItemsOffer _offer;

        public AddItemToUnitOperation(IItemView item1, IItemView item2, IGridView gridView, Action<EMergeResult,bool> callback)
        {
            _callback = callback;
            _gridView = gridView;
            if (item1.itemData.core.type == MergeConstants.TypeUnits)
            {
                _unitView = item1;
                _itemView = item2;
                _oneIntoTwo = false;
            }
            else
            {
                _unitView = item2;
                _itemView = item1;
                _oneIntoTwo = true;
            }
        }

        public void Process()
        {
            var itemContainerInto = _unitView.Transform.gameObject.GetComponent<IHeroItemsContainer>();
            if (itemContainerInto == null)
            {
                CLog.LogRed($"NO <IUnitsItemsContainer> on {_unitView.Transform.gameObject.name}");
                return;
            }
            var currentItems = itemContainerInto.Items;
            // var newItem = _itemView.itemData.core;
            var newItem = new HeroItemData(_itemView.Transform.gameObject);
            var didMerge = false;
            var replaceIndex = 0;
            var mergedItem = (HeroItemData)null;
            for (var i = 0; i < currentItems.Count; i++)
            {
                var item = currentItems[i];
                if (item.id == newItem.id && 
                    item.level == newItem.level &&
                    item.level < MergeConstants.MaxItemLevel)
                {
                    var core = new CoreItemData(){
                        id = newItem.id,
                        level = newItem.level + 1,
                        type = newItem.type };
                    mergedItem = new HeroItemData(core, newItem.modifierIds);
                    mergedItem.modifierIds.AddRange(item.modifierIds);
                    
                    currentItems[i] = mergedItem;
                    replaceIndex = i;
                    MergeFunctions.ClearCellAndHideItem(_gridView, _itemView);
                    didMerge = true;
                    break;
                }
            }

            if (didMerge)
            {
                var count = currentItems.Count;
                var mergedItems = MergeFunctions.TryMergeAll(currentItems, MergeConstants.MaxItemLevel);
                if (mergedItems.Count == count) // nothing changed
                    itemContainerInto.ReplaceWithMergedItem(replaceIndex, mergedItem);
                else // merged with other items
                    itemContainerInto.UpdateItems(mergedItems);
                ProcessItemsPositions();
                Complete();
                return;
            }
            
            if (currentItems.Count < MergeConstants.MaxItemsCount)
            {
                itemContainerInto.AddNewItem(newItem);
                ProcessItemsPositions();
                Complete();
                return;
            }
            var allItems = new List<HeroItemData>(MergeConstants.MaxItemsCount * 2);
            allItems.AddRange(currentItems);
            allItems.Add(newItem);
            _offer = new ChooseItemsOffer(MergeConstants.MaxItemsCount, this);
            _offer.OfferChooseItems(allItems);
        }

        private void ProcessItemsPositions()
        {
            if (_oneIntoTwo) // dragged into standing. Item into Unit
            {
                MergeFunctions.ClearCell(_gridView, _itemView);
                _itemView.Hide();
            }
            else // standing into dragged. Unit into item
            {
                MergeFunctions.ClearCell(_gridView, _unitView);
                var targetCell = _gridView.GetCell(_itemView.itemData.pivotX, _itemView.itemData.pivotY);
                MergeFunctions.ClearCellAndHideItem(_gridView, _itemView);
                MergeFunctions.PutItemToCell(_unitView, targetCell);
            }
        }
        
        public void ConfirmChosenItems(List<HeroItemData> allItems, List<int> chosen, List<int> left)
        {
            var chosenItems = new List<HeroItemData>(allItems.Count);
            foreach (var ind in chosen)
                chosenItems.Add(allItems[ind]);
            if (_oneIntoTwo) // dragged into standing. Unit is standing
            {
                var container = _unitView.Transform.gameObject.GetComponent<IHeroItemsContainer>();
                container.SetItems(chosenItems);
                MergeFunctions.ClearCellAndHideItem(_gridView, _itemView);
            }
            else // standing into dragged. Unit is dragged
            {
                var container = _unitView.Transform.gameObject.GetComponent<IHeroItemsContainer>();
                container.SetItems(chosenItems);
                MergeFunctions.ClearCell(_gridView, _unitView);
                var targetCell = _gridView.GetCell(_itemView.itemData.pivotX, _itemView.itemData.pivotY);
                MergeFunctions.ClearCellAndHideItem(_gridView, _itemView);
                MergeFunctions.PutItemToCell(_unitView, targetCell);
            }
            var spawner = ServiceLocator.Get<IMergeItemsFactory>();
            var cellPicker = ServiceLocator.Get<IGridSectionsController>();
            foreach (var ind in left)
            {
                var itemData = new ItemData(allItems[ind].core);
                var hasCell = cellPicker.GetFreeAllowedCell(_gridView.BuiltGrid, itemData, out var coords);
                if (!hasCell)
                {
                    CLog.Log($"[{nameof(ClassBasedMergeProcessor)}] No more free cells to put items");
                    break;
                }
                var cell = _gridView.GetCell(coords.x, coords.y);
                spawner.SpawnItemOnCell(cell, itemData); // possible MODIFY THIS !!
            }
            Complete();
        }
        
        private void Complete()
        {
            _callback.Invoke(EMergeResult.MergedOneIntoAnother, _oneIntoTwo);
        }
    }
}