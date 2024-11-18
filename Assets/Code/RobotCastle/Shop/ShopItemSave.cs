using RobotCastle.Data;

namespace RobotCastle.Shop
{
    [System.Serializable]
    public class ShopItemSave
    {
        public bool isAvailable;
        public CoreItemData itemData;
        
        public ShopItemSave(){}

        public ShopItemSave(ShopItemSave other)
        {
            isAvailable = other.isAvailable;
            itemData = new CoreItemData(other.itemData);
        }
    }
}