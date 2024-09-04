using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitsItemsContainer : MonoBehaviour, IUnitsItemsContainer
    {
        private const int MaxSize = 3;
        [SerializeField] private UnitItemsContainerView _view;
        private readonly List<CoreItemData> _items = new (MaxSize);

        public int MaxCount => GlobalConfig.MaxUnitsItemsCount;
        
        public int ItemsCount => _items.Count;
        
        public List<CoreItemData> Items => _items;

        public void SetItems(List<CoreItemData> items)
        {
            if (_items != items)
            {
                _items.Clear();
                foreach (var it in items)
                    _items.Add(it);
            }
            _view.ShowItems(items);
        }
        
        public void ReplaceWithMergedItem(int indexAt, CoreItemData newItem)
        {
            CLog.LogGreen($"ReplaceWithMergedItem. {indexAt}. {newItem.ItemDataStr()}");
            if (indexAt >= _items.Count)
            {
                _items.Add(newItem);
                
            }
            else
            {
                _items[indexAt] = newItem;
            }
            _view.UpdateMergedItem(_items, indexAt);
        }

        public void AddNewItem(CoreItemData newItem)
        {
            CLog.LogGreen($"AddNewItem. {newItem.ItemDataStr()}");
            _items.Add(newItem);
            _view.ShowLastAddedItem(_items);
        }

        public void UpdateView()
        {
            _view.ShowItems(_items);
        }
    }
}