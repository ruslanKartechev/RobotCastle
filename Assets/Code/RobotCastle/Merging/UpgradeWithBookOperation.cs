using System;
using RobotCastle.Battling;
using RobotCastle.Core;

namespace RobotCastle.Merging
{
    public class UpgradeWithBookOperation
    {
        private IItemView _bookItem;
        private IItemView _anotherItem;
        private IGridView _gridView;
        private IMergeItemsContainer _itemsContainer;
        private IMergeMaxLevelCheck _maxLevelCheck;
        private Action<EMergeResult, bool> _callback;
        private bool _isBookTaken;

        public UpgradeWithBookOperation(IItemView bookItem, 
            IItemView anotherItem, bool bookTaken, IGridView gridView,
            IMergeItemsContainer itemsContainer, IMergeMaxLevelCheck maxLevelCheck, Action<EMergeResult, bool> callback)
        {
            _bookItem = bookItem;
            _anotherItem = anotherItem;
            _gridView = gridView;
            _callback = callback;
            _isBookTaken = bookTaken;
            _itemsContainer = itemsContainer;
            _maxLevelCheck = maxLevelCheck;
        }

        public void Process()
        {
            var anotherData = _anotherItem.itemData.core;
            if (_maxLevelCheck.CanUpgradeFurther(anotherData) == false)
            {
                Failed();
                return;
            }
            if (anotherData.type == _bookItem.itemData.core.type)
            {
                UpgradeAsItem();
                _callback.Invoke(EMergeResult.MergedOneIntoAnother, _isBookTaken);
                return;
            }
            
            var upgItem = _bookItem.Transform.gameObject.GetComponent<IUpgradeItem>();
            if (!upgItem.CanUpgrade(anotherData))
            {
                Failed();
                return;
            }
            
            switch (anotherData.type)
            {
                case MergeConstants.TypeWeapons:
                    UpgradeAsItem();             
                    break;
                case MergeConstants.TypeHeroes:
                    UpgradeAsUnit();
                    break;
                default:
                    Failed();
                    break;
            }
            _callback.Invoke(EMergeResult.MergedOneIntoAnother, _isBookTaken);

            void UpgradeAsUnit()
            {
                if (!_isBookTaken)
                {
                    var x = _bookItem.itemData.pivotX;
                    var y = _bookItem.itemData.pivotY;
                    MergeFunctions.MoveToAnotherCell(_gridView, _anotherItem, x,y);
                    _bookItem.Hide();
                }
                else
                    MergeFunctions.ClearCellAndHideItem(_gridView, _bookItem);

                MergeFunctions.AddLevelToItem(_anotherItem);
                _itemsContainer.RemoveItem(_bookItem);
            }            
            
            void UpgradeAsItem()
            {
                IItemView taken = null;
                IItemView standing = null;
                
                if (_isBookTaken)
                {
                    taken = _bookItem;
                    standing = _anotherItem;
                }
                else
                {
                    taken = _anotherItem;
                    standing = _bookItem;
                }
                var x = standing.itemData.pivotX;
                var y = standing.itemData.pivotY;
                _itemsContainer.RemoveItem(standing);
                _itemsContainer.RemoveItem(taken);
                MergeFunctions.ClearCellAndHideItem(_gridView, standing);
                anotherData.level++;
                var newItem = ServiceLocator.Get<IMergeItemsFactory>().SpawnItemOnCell(_gridView.GetCell(x, y), new ItemData(anotherData));      
                MergeFunctions.ClearCellAndHideItem(_gridView, taken);
                _itemsContainer.AddNewItem(newItem);
            }
        }

        private void Failed()
        {
            _callback?.Invoke(EMergeResult.NoMerge, true);
        }
    }
}