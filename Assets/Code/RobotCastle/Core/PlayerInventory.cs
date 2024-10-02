using System.Collections.Generic;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class PlayerInventory
    {
        public List<InventoryItemData> items;
        
        public PlayerInventory(){}

        public PlayerInventory(PlayerInventory other)
        {
            var count = other.items.Count;
            items = new(count);
            for (int i = 0; i < count; i++)
            {
                items.Add(new InventoryItemData(other.items[i]));
            }
        }
    }
}