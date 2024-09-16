using System;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;

namespace RobotCastle.Merging
{
    public class UpgradeWithBookOperation
    {
        private IItemView _bookItem;
        private IItemView _anotherItem;
        private IGridView _gridView;
        private Action<EMergeResult, bool> _callback;
        private bool _isBookTaken;

        public UpgradeWithBookOperation(IItemView bookItem, IItemView anotherItem, bool bookTaken, IGridView gridView, Action<EMergeResult, bool> callback)
        {
            _bookItem = bookItem;
            _anotherItem = anotherItem;
            _gridView = gridView;
            _callback = callback;
            _isBookTaken = bookTaken;
        }

        public void Process()
        {
            var anotherData = _anotherItem.itemData.core;
            var maxLvl = ServiceLocator.Get<ViewDataBase>().GetMaxMergeLevel(anotherData.id);
            if (anotherData.level >= maxLvl)
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
                case MergeConstants.TypeItems:
                    UpgradeAsItem();             
                    break;
                case MergeConstants.TypeUnits:
                    UpgradeAsUnit();
                    break;
                default:
                    Failed();
                    break;
            }
            _callback.Invoke(EMergeResult.MergedOneIntoAnother, _isBookTaken);

            void UpgradeAsUnit()
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
                anotherData.level++;
                standing.UpdateViewToData();
                standing.OnMerged();
                MergeFunctions.ClearCellAndHideItem(_gridView, taken);
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
                MergeFunctions.ClearCellAndHideItem(_gridView, standing);
                anotherData.level++;
                ServiceLocator.Get<IMergeItemsFactory>().SpawnItemOnCell(_gridView.GetCell(x, y), new ItemData(anotherData));      
                MergeFunctions.ClearCellAndHideItem(_gridView, taken);
            }
        }

        private void Failed()
        {
            _callback?.Invoke(EMergeResult.NoMerge, true);
        }
    }
}