using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Shop
{
    public enum EPurchaseResult {Success, NotEnoughMoney, Error}

    public class ShopManager : IShopManager
    {
        public bool GrandItem(CoreItemData item)
        {
            switch (item.type)
            {
                case ItemsIds.TypeHeroes:
                    
                    break;
                case ItemsIds.TypeBonus:
                                        
                    break;
                default:
                    CLog.LogError($"Unknown item type: {item.type}");
                    return false;
            }
            return true;
        }

        
        
    }
}