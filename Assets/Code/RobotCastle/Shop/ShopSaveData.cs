using System.Collections.Generic;
using RobotCastle.Core;

namespace RobotCastle.Shop
{
    [System.Serializable]
    public class ShopSaveData
    {
        public DateTimeData dailyOfferStartTime;
        public List<ShopItemSave> dailyItems;
        
        public ShopSaveData(){}

        public ShopSaveData(ShopSaveData other)
        {
            dailyOfferStartTime = new DateTimeData(other.dailyOfferStartTime);
            var count = other.dailyItems.Count;
            dailyItems = new(count);
            for (var i = 0; i < count; i++)
            {
                dailyItems.Add(new ShopItemSave(other.dailyItems[i]));
            }
        }
    }
}