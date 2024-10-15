using System;
using System.Collections.Generic;
using RobotCastle.Core;

namespace RobotCastle.Merging
{
    public class MergeWeaponsOperation
    {
        private Action<EMergeResult, bool> _callback;
        private IItemView _itemMoving;
        private IItemView _itemStanding;
        private IGridView _gridView;
        private IMergeItemsContainer _container;
        private IMergeMaxLevelCheck _maxLevelCheck;
        private List<IMergeModifier> _modifiers;

        
        public MergeWeaponsOperation(IItemView itemMoving, IItemView itemStanding, 
            IGridView gridView, IMergeItemsContainer container, 
            IMergeMaxLevelCheck maxLevelCheck, Action<EMergeResult,bool> callback,
            List<IMergeModifier> modifiers)
        {
            _itemMoving = itemMoving;
            _itemStanding = itemStanding;
            _gridView = gridView;
            _callback = callback;
            _container = container;
            _maxLevelCheck = maxLevelCheck;
            _modifiers = modifiers;
        }

        public void Process()
        {
            var itemInto = _itemStanding.itemData;
            if (_maxLevelCheck.CanUpgradeFurther(itemInto.core) == false)
            {
                _callback.Invoke(EMergeResult.NoMerge, true);
                return;
            }
            
            var mergedItem = new ItemData(itemInto);
            mergedItem.core.level++;
            _container.RemoveItem(_itemMoving);
            _container.RemoveItem(_itemStanding);
            MergeFunctions.ClearCellAndHideItem(_gridView, _itemMoving);
            MergeFunctions.ClearCellAndHideItem(_gridView, _itemStanding);
                                
            var spawner = ServiceLocator.Get<IMergeItemsFactory>();
            var cell = _gridView.Grid[itemInto.pivotX, itemInto.pivotY];
            var newItemView = spawner.SpawnItemOnCell(cell, mergedItem);
            _container.AddNewItem(newItemView);

            foreach (var mod in _modifiers)
                mod.OnNewItemSpawnDuringMerge(newItemView, _itemStanding, _itemMoving);
            
            MergeFunctions.PlayMergeFX(_itemStanding);
            _callback?.Invoke(EMergeResult.MergedIntoNew, true);
        }
    }
}