using System.Collections.Generic;

namespace RobotCastle.Shop
{
    [System.Serializable]
    public class ShopDataBase
    {
        public List<ShopItemData> dailyOffers;
        public List<ShopItemData> resources;
        public List<ShopItemData> otherOffers;
        
    }
}