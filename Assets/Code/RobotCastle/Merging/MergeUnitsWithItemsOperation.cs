using System;
using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Merging
{
    public class MergeUnitsWithItemsOperation : IItemsChoiceListener
    {
        private const int MaxItemLevel = 3;
        private const int MaxItemsCount = 3;
        
        private Action<EMergeResult, bool> _callback;
        private bool _oneIntoTwo;
        private IItemView _unitStanding;
        private IItemView _unitMoving;
        private IGridView _gridView;
        private ChooseItemsOffer _offer;

        public MergeUnitsWithItemsOperation(IItemView unitMoving, IItemView unitStanding, IGridView gridView, Action<EMergeResult,bool> callback)
        {
            _callback = callback;
            _gridView = gridView;
            _unitStanding = unitStanding;
            _unitMoving = unitMoving;
            _oneIntoTwo = true;
        }

        public void Process()
        {
            var cont1 = _unitStanding.Transform.GetComponent<IUnitsItemsContainer>();
            var cont2 = _unitMoving.Transform.GetComponent<IUnitsItemsContainer>();
            if (cont1.ItemsCount + cont2.ItemsCount == 0)
            {
                SetPositions();
                Complete();
                return;
            }
            
            var allItems = new List<CoreItemData>(6);
            allItems.AddRange(cont1.Items);
            allItems.AddRange(cont2.Items);
            var it = 0;
            const int itMax = 100;
            while (it < itMax && TryAdd())
            {
                it++;
            }
 
            allItems.RemoveNulls();
            if (allItems.Count > MaxItemsCount)
            {
                _offer = new ChooseItemsOffer(MaxItemsCount, this);
                _offer.OfferChooseItems(allItems);
            }
            else
            {
                cont1.UpdateItems(allItems);
                SetPositions();
                Complete();
            }
            
            bool TryAdd()
            {
                var count = allItems.Count;
                for (var indOne = count - 1; indOne >= 1; indOne--)
                {
                    if (allItems[indOne] == null)
                        continue;
                    if (allItems[indOne].level >= MaxItemLevel)
                        continue;
                    var itemOne = allItems[indOne];
                    for (var indTwo = indOne - 1; indTwo >= 0; indTwo--)
                    {
                        if (allItems[indTwo] == null)
                            continue;
                        var itemTwo = allItems[indTwo];
                        if (itemOne == itemTwo)
                        {
                            itemOne.level++;
                            // CLog.Log($"Merge: {itemOne.ItemDataStr()} INTO {itemTwo.ItemDataStr()}");
                            allItems[indTwo] = null;
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        
        public void ConfirmChosenItems(List<CoreItemData> chosen, List<CoreItemData> left)
        {
            _unitStanding.Transform.GetComponent<IUnitsItemsContainer>().UpdateItems(chosen);
            var spawner = ServiceLocator.Get<IMergeItemsFactory>();
            var cellPicker = ServiceLocator.Get<IGridSectionsController>();
            foreach (var coreData in left)
            {
                var hasCell = cellPicker.GetFreeCell(_gridView.BuiltGrid, out var coords);
                if (!hasCell)
                    break;
                var cell = _gridView.GetCell(coords.x, coords.y);
                spawner.SpawnItemOnCell(cell, new ItemData(coreData));
            }
            SetPositions();
            Complete();
        }

        private void SetPositions()
        {
            MergeFunctions.ClearCellAndHideItem(_gridView, _unitMoving);
        }
        
        private void Complete()
        {
            _callback.Invoke(EMergeResult.MergedOneIntoAnother, _oneIntoTwo);
        }
    }
}