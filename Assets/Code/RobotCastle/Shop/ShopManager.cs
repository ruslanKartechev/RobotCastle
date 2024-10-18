using System;
using SleepDev;

namespace RobotCastle.Shop
{
    public class ShopManager
    {
        public void PurchaseItem(ShopItemData itemData, Action callback)
        {
            CLog.LogWhite($"[ShopManager] Trying to purchase: {itemData.itemData.id}, amount: {itemData.itemData.level}, price: {itemData.cost}");
            callback?.Invoke();
        }
    }
}