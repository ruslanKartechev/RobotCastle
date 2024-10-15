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
        private IItemView _hero;
        private IItemView _weapon;
        private IGridView _gridView;
        private IMergeItemsContainer _container;
        private ChooseItemsOffer _offer;
        private List<IMergeModifier> _modifiers;

        public AddItemToUnitOperation(IItemView item1, IItemView item2, IGridView gridView, IMergeItemsContainer container, Action<EMergeResult,bool> callback, List<IMergeModifier> modifiers)
        {
            _callback = callback;
            _gridView = gridView;
            _container = container;
            _modifiers = modifiers;
            if (item1.itemData.core.type == MergeConstants.TypeHeroes)
            {
                _hero = item1;
                _weapon = item2;
                _oneIntoTwo = false;
            }
            else
            {
                _hero = item2;
                _weapon = item1;
                _oneIntoTwo = true;
            }
        }

        public void Process()
        {
            var itemContainerInto = _hero.Transform.gameObject.GetComponent<IHeroWeaponsContainer>();
            if (itemContainerInto == null)
            {
                CLog.LogRed($"NO <IUnitsItemsContainer> on {_hero.Transform.gameObject.name}");
                return;
            }
            var currentItems = itemContainerInto.Items;
            // var newItem = _itemView.itemData.core;
            var newItem = new HeroWeaponData(_weapon.Transform.gameObject);
            var didMerge = false;
            var replaceIndex = 0;
            var mergedItem = (HeroWeaponData)null;
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
                    mergedItem = new HeroWeaponData(core, newItem.modifierIds);
                    mergedItem.modifierIds.AddRange(item.modifierIds);
                    
                    currentItems[i] = mergedItem;
                    replaceIndex = i;
                    _container.RemoveItem(_weapon);
                    MergeFunctions.ClearCellAndHideItem(_gridView, _weapon);
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
            var allItems = new List<HeroWeaponData>(MergeConstants.MaxItemsCount * 2);
            allItems.AddRange(currentItems);
            allItems.Add(newItem);
            _offer = new ChooseItemsOffer(MergeConstants.MaxItemsCount, this);
            _offer.OfferChooseItems(allItems);
        }

        private void ProcessItemsPositions()
        {
            if (_oneIntoTwo) // dragged into standing. Item into Unit
            {
                MergeFunctions.ClearCell(_gridView, _weapon);
                _weapon.Hide();
            }
            else // standing into dragged. Unit into item
            {
                MergeFunctions.ClearCell(_gridView, _hero);
                var targetCell = _gridView.GetCell(_weapon.itemData.pivotX, _weapon.itemData.pivotY);
                MergeFunctions.ClearCellAndHideItem(_gridView, _weapon);
                MergeFunctions.PutItemToCell(_hero, targetCell);
            }
        }
        
        public void ConfirmChosenItems(List<HeroWeaponData> allWeapons, List<int> chosen, List<int> left)
        {
            var chosenItems = new List<HeroWeaponData>(allWeapons.Count);
            foreach (var ind in chosen)
                chosenItems.Add(allWeapons[ind]);
            if (_oneIntoTwo) // dragged into standing. Unit is standing
            {
                var container = _hero.Transform.gameObject.GetComponent<IHeroWeaponsContainer>();
                container.SetItems(chosenItems);
                MergeFunctions.ClearCellAndHideItem(_gridView, _weapon);
            }
            else // standing into dragged. Unit is dragged
            {
                var container = _hero.Transform.gameObject.GetComponent<IHeroWeaponsContainer>();
                container.SetItems(chosenItems);
                MergeFunctions.ClearCell(_gridView, _hero);
                var targetCell = _gridView.GetCell(_weapon.itemData.pivotX, _weapon.itemData.pivotY);
                MergeFunctions.ClearCellAndHideItem(_gridView, _weapon);
                MergeFunctions.PutItemToCell(_hero, targetCell);
            }
            var spawner = ServiceLocator.Get<IMergeItemsFactory>();
            var cellPicker = ServiceLocator.Get<IGridSectionsController>();
            foreach (var ind in left)
            {
                var itemData = new ItemData(allWeapons[ind].core);
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
            foreach (var mod in _modifiers)
                mod.OnMergedOneIntoAnother(_weapon, _hero);
            
            _callback.Invoke(EMergeResult.MergedOneIntoAnother, _oneIntoTwo);
        }
    }
}