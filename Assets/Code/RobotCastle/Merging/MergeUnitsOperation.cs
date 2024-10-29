using System;
using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;

namespace RobotCastle.Merging
{
    public class MergeUnitsOperation : IItemsChoiceListener
    {
        
        private Action<EMergeResult, bool> _callback;
        private IItemView _unitStanding;
        private IItemView _unitMoving;
        private IGridView _gridView;
        private IMergeItemsContainer _container;
        private IMergeMaxLevelCheck _maxLevelCheck;
        private int _maxWeaponsCount = 3;
        private ChooseItemsOffer _offer;
        private List<IMergeModifier> _modifiers;

        public MergeUnitsOperation(IItemView unitMoving, IItemView unitStanding, 
            IGridView gridView, IMergeItemsContainer container, IMergeMaxLevelCheck maxLevelCheck, 
            Action<EMergeResult,bool> callback, List<IMergeModifier> modifiers)
        {
            _callback = callback;
            _gridView = gridView;
            _unitStanding = unitStanding;
            _unitMoving = unitMoving;
            _container = container;
            _maxLevelCheck = maxLevelCheck;
            _modifiers = modifiers;
        }

        public void Process()
        {
            if (_maxLevelCheck.CanUpgradeFurther(_unitStanding.itemData.core) == false)
            {
                _callback.Invoke(EMergeResult.NoMerge, true);
                return;
            }
            var cont1 = _unitStanding.Transform.GetComponent<IHeroWeaponsContainer>();
            var cont2 = _unitMoving.Transform.GetComponent<IHeroWeaponsContainer>();
            if (cont1.ItemsCount + cont2.ItemsCount == 0)
            {
                // SetPositions();
                Complete();
                return;
            }
            _maxWeaponsCount = cont1.MaxCount;
            
            var allWeapons = new List<HeroWeaponData>(6);
            allWeapons.AddRange(cont1.Items);
            allWeapons.AddRange(cont2.Items);
            var it = 0;
            const int itMax = 100;
            while (it < itMax && TryAdd())
            {
                it++;
            }

            if (it >= itMax)
            {
                CLog.LogError($"It >= itMax ({itMax}). ERROR adding items!");
            }
 
            allWeapons.RemoveNulls();
            if (allWeapons.Count > _maxWeaponsCount)
            {
                _offer = new ChooseItemsOffer(_maxWeaponsCount, this);
                _offer.OfferChooseItems(allWeapons);
            }
            else
            {
                cont1.UpdateItems(allWeapons);
                // SetPositions();
                Complete();
            }
            
            bool TryAdd()
            {
                var count = allWeapons.Count;
                for (var indOne = count - 1; indOne >= 1; indOne--)
                {
                    if (allWeapons[indOne] == null)
                        continue;
                    if(_maxLevelCheck.CanUpgradeFurther(allWeapons[indOne].core) == false)
                        continue;
                    var w1 = allWeapons[indOne];
                    for (var indTwo = indOne - 1; indTwo >= 0; indTwo--)
                    {
                        if (allWeapons[indTwo] == null)
                            continue;
                        var w2 = allWeapons[indTwo];
                        if (w1 == w2)
                        {
                            w1.core.level++;
                            // CLog.Log($"Merge: {itemOne.ItemDataStr()} INTO {itemTwo.ItemDataStr()}");
                            allWeapons[indTwo] = null;
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        
        public void ConfirmChosenItems(List<HeroWeaponData> allWeapons, List<int> chosenIndices, List<int> leftIndices)
        {
            var chosen = new List<HeroWeaponData>(chosenIndices.Count);
            foreach (var ind in chosenIndices)
                allWeapons.Add(allWeapons[ind]);   
            _unitStanding.Transform.GetComponent<IHeroWeaponsContainer>().UpdateItems(chosen);
            var spawner = ServiceLocator.Get<IMergeItemsFactory>();
            var cellPicker = ServiceLocator.Get<IGridSectionsController>();
            foreach (var ind in leftIndices)
            {
                var itemData = new ItemData(allWeapons[ind].core);
                var hasCell = cellPicker.GetFreeAllowedCell(_gridView.BuiltGrid, itemData, out var coords);
                if (!hasCell)
                    break;
                var cell = _gridView.GetCell(coords.x, coords.y);
                spawner.SpawnItemOnCell(cell, itemData);
            }
            // SetPositions();
            Complete();
        }

        private void SetPositions()
        {
            // _container.RemoveItem(_unitMoving);
            MergeFunctions.ClearCellAndHideItem(_gridView, _unitMoving);
        }
        
        private void Complete()
        {
            _container.RemoveItem(_unitMoving);

            _unitStanding.itemData.core.level++;
            ServiceLocator.Get<MergeAnimation>().Play(_unitStanding, _unitMoving, _gridView, AnimationCallback);
        }

        private void AnimationCallback()
        {
            foreach (var mod in _modifiers)
                mod.OnMergedOneIntoAnother(_unitMoving, _unitStanding);
            _callback.Invoke(EMergeResult.MergedOneIntoAnother, true); // always merges ItemMoving into ItemTaken
            var cell = _gridView.GetCell(_unitStanding.itemData.pivotX, _unitStanding.itemData.pivotY);
            _unitStanding.MoveToPoint(cell.ItemPoint, MergeConstants.MergeItemPutAnimationTime);
        }
    }
}