namespace RobotCastle.Core
{
    [System.Serializable]
    public class InventoryItemData
    {
        public string id;
        public int amount;
        
        public InventoryItemData(){}

        public InventoryItemData(InventoryItemData other)
        {
            id = other.id;
            amount = other.amount;
        }
    }
}