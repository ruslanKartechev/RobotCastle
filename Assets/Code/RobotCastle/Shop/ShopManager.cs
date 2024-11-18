using System;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
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

        public async Task<EPurchaseResult> Purchase(EShopCurrency currencyType, int cost, CancellationToken token)
        {
            switch (currencyType)
            {
                case EShopCurrency.Money:
                    return SubCost(cost, false);
                case EShopCurrency.HardMoney:
                    return SubCost(cost, true);
                case EShopCurrency.AdsReward:

                    break;
                case EShopCurrency.RealWorldMoney:
                    
                    break;
            }
            return EPurchaseResult.Success;
        }

        private EPurchaseResult SubCost(int amount, bool hard)
        {
            var gm = ServiceLocator.Get<GameMoney>();
            if (hard)
            {
                var owned = gm.globalHardMoney.Val;
                if (owned < amount)
                    return EPurchaseResult.NotEnoughMoney;
                gm.globalHardMoney.AddValue(-amount);
            }
            else
            {
                var owned = gm.globalMoney.Val;
                if (owned < amount)
                    return EPurchaseResult.NotEnoughMoney;
                gm.globalMoney.AddValue(-amount);
            }            
            return EPurchaseResult.Success;    
        }

        private async Task<EPurchaseResult> PurchaseForAds(int placementId, CancellationToken token)
        {
            var placement = ServiceLocator.Get<InAppsSKUDataBase>().consumables;
            return EPurchaseResult.Error;
        }

     
    }
}