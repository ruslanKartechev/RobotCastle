﻿using System.Collections.Generic;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroItemsContainer : MonoBehaviour, IHeroItemsContainer
    {
        public int MaxCount => GlobalConfig.MaxUnitsItemsCount;
        public int ItemsCount => _items.Count;
        public List<HeroItemData> Items => _items;
        
        private const int MaxSize = 3;
        
        [SerializeField] private HeroView _view;
        private readonly List<HeroItemData> _items = new (MaxSize);

        public void UpdateItems(List<HeroItemData> items)
        {
            if (_items != items)
            {
                _items.Clear();
                foreach (var it in items)
                    _items.Add(it);
            }
            _view.heroUI.Items.ShowItems(items);
            _view.heroUI.Items.Animate();
        }

        public void SetItems(List<HeroItemData> items)
        {
            if (_items != items)
            {
                _items.Clear();
                foreach (var it in items)
                    _items.Add(it);
            }
            _view.heroUI.Items.ShowItems(items);
        }
        
        public void ReplaceWithMergedItem(int indexAt, HeroItemData newItem)
        {
            // CLog.LogGreen($"ReplaceWithMergedItem. {indexAt}. {newItem.AsStr()}");
            if (indexAt >= _items.Count)
                _items.Add(newItem);
            else
                _items[indexAt] = newItem;
            _view.heroUI.Items.UpdateMergedItem(_items, indexAt);
            _view.heroUI.Items.Animate();
        }

        public void AddNewItem(HeroItemData newItem)
        {
            // CLog.LogGreen($"AddNewItem. {newItem.AsStr()}");
            _items.Add(newItem);
            _view.heroUI.Items.ShowLastAddedItem(_items);
            _view.heroUI.Items.Animate();
        }

        public void UpdateView()
        {
            _view.heroUI.Items.ShowItems(_items);
        }

        [ContextMenu("LogAllModifiers")]
        public void LogAllModifiers()
        {
            var msg = $"[{gameObject.name}] Modifiers: ";
            foreach (var item in _items)
            {
                foreach (var id in item.modifierIds)
                {
                    msg += id + ", ";
                }
            }
            CLog.LogGreen(msg);
        }

        public void AddAllModifiersToHero()
        {
            var db = ServiceLocator.Get<ModifiersDataBase>();
            foreach (var item in _items)
            {
                foreach (var id in item.modifierIds)
                {
                    var mod = db.GetModifier(id);
                    mod.AddTo(gameObject);
                }
            }
        }
        
    }
}