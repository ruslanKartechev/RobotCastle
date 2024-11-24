using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class PlayerInventory
    {
        /// <summary>
        /// Don't use this to read items. Use the "itemsMap" Dictionary!
        /// </summary>
        public List<InventoryItemData> items;
        public List<ScrollSave> scrolls;
        
        [NonSerialized] public Dictionary<string, InventoryItemData> itemsMap = new (10);
        [NonSerialized] private Dictionary<string, ScrollSave> scrollsMap = new(10);

        
        public PlayerInventory(){}

        public PlayerInventory(PlayerInventory other)
        {
            var count = other.items.Count;
            items = new(count);
            itemsMap = new (count);
            for (int i = 0; i < count; i++)
            {
                var it = new InventoryItemData(other.items[i]);
                items.Add(it);
            }
            count = other.scrolls.Count;
            scrolls = new (count);
            scrollsMap = new (count);
            for (var i = 0; i < count; i++)
            {
                var it = new ScrollSave(other.scrolls[i]);
                scrolls.Add(it);
            }
        }

        public void Log()
        {
            CLog.LogYellow($"==============");
            CLog.LogYellow($"Inventory items count: {items.Count}, Scrolls count: {scrolls.Count}");
            foreach (var it in items)
                CLog.Log($"Item: {it.id}, {it.amount}");
            foreach (var it in scrolls)
                CLog.Log($"Item: {it.id}, {it.ownedAmount}");
            CLog.LogYellow($"==============");
        }

        public void InitAfterLoad()
        {
            foreach (var it in items)
                itemsMap.Add(it.id, it);

            foreach (var it in scrolls)
                scrollsMap.Add(it.id, it);
        }
        
        public void PrepareForSave()
        {
            items.Clear();
            items.Capacity = itemsMap.Count;
            foreach (var (id, ss) in itemsMap)
                items.Add(ss);
            
            scrolls.Clear();
            scrolls.Capacity = scrollsMap.Count;
            foreach (var (id, ss) in scrollsMap)
                scrolls.Add(ss);
            
            // CLog.LogYellow($"When saving, map count: {scrollsMap.Count}, list count: {scrolls.Count}.  Inventory: {items.Count}, InventoryMap: {itemsMap.Count}");
            // foreach (var it in scrolls)
                // CLog.LogYellow($"SAVING Scroll: {it.id}, amount: {it.ownedAmount}");
        }

        /// <summary>
        /// If item is not contained -> will add the item with 0 count
        /// </summary>
        /// <param name="id">id of the item</param>
        /// <returns>amount of [id] items in inventory </returns>
        public int GetCount(string id)
        {
            if (itemsMap.ContainsKey(id) == false)
            {
                itemsMap.Add(id, new InventoryItemData(id, 0));
                return 0;
            }
            return itemsMap[id].amount;
        }

        public void SetCount(string id, int amount)
        {
            if (itemsMap.ContainsKey(id) == false)
            {
                itemsMap.Add(id, new InventoryItemData(id, amount));
                return;
            }
            itemsMap[id].amount = amount;
        }

        public void AddItem(string id, int count)
        {
            CLog.Log($"[Inventory] Adding item: {id}, count: {count}");
            if (id.Contains("scroll_tier"))
            {
                AddScrollsCount(id, count);
                return;
            }
            if (itemsMap.ContainsKey(id) == false)
            {
                itemsMap.Add(id, new InventoryItemData(id, count));
                return;
            }
            itemsMap[id].amount += count;
        }
        
        public ScrollSave GetScrollSave(string id)
        {
            if (scrollsMap.ContainsKey(id))
                return scrollsMap[id];
            var save = new ScrollSave()
            {
                id = id,
                ownedAmount = 0,
                purchasedCount = 0,
                timerData = new DateTimeData()
            };
            scrollsMap.Add(id,save);
            return save;
        }

        public int AddScrollsCount(string id, int amount)
        {
            if (scrollsMap.ContainsKey(id))
            {
                scrollsMap[id].ownedAmount += amount;
                return amount;
            }
            scrollsMap.Add(id, new ScrollSave(id, amount));
            return amount;
        }
        
        public int SetScrollsCount(string id, int amount)
        {
            if (scrollsMap.ContainsKey(id))
            {
                scrollsMap[id].ownedAmount = amount;
                return amount;
            }
            scrollsMap.Add(id, new ScrollSave(id, amount));
            return amount;
        }

        
        /// <summary>
        /// USE IN EDITOR ONLY
        /// </summary>
        public void Reset()
        {
            itemsMap.Clear();
            foreach (var scroll in scrolls)
            {
                scroll.ownedAmount = scroll.purchasedCount = 0;
            }
        }
    }
}