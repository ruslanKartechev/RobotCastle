using RobotCastle.Data;

namespace RobotCastle.Shop
{
    [System.Serializable]
    public class ShopItemData
    {
        public string sku;
        public int cost;
        public EShopCurrency currency; 
        public CoreItemData itemData;
    }
}