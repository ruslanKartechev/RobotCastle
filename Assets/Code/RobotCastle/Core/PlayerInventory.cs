using System.Collections.Generic;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class PlayerInventory
    {
        public List<InventoryItemData> items;
        public Dictionary<string, InventoryItemData> itemsMap = new ();

        public PlayerInventory(){}

        public PlayerInventory(PlayerInventory other)
        {
            var count = other.items.Count;
            items = new(count);
            itemsMap = new Dictionary<string, InventoryItemData>(count);
            for (int i = 0; i < count; i++)
            {
                var it = new InventoryItemData(other.items[i]);
                items.Add(it);
                itemsMap.Add(it.id, it);
            }

        }
    }
}