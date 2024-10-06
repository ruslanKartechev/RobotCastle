using System.Collections.Generic;
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
        [Space(5)]
        public List<ScrollSave> scrolls;
        public Dictionary<string, InventoryItemData> itemsMap = new (10);
        public Dictionary<string, ScrollSave> scrollsMap = new(10);

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
                itemsMap.Add(it.id, it);
            }
            count = other.scrolls.Count;
            scrolls = new (count);
            scrollsMap = new (count);
            for (var i = 0; i < count; i++)
            {
                var it = new ScrollSave(other.scrolls[i]);
                scrolls.Add(it);
                scrollsMap.Add(it.id, it);
            }
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

        public int GetScrollsCount(string id)
        {
            if (scrollsMap.ContainsKey(id))
                return scrollsMap[id].ownedAmount;
            return 0;
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
    }
}