using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Data;

namespace RobotCastle.Shop
{
    public interface IShopManager
    {
        Task<EPurchaseResult> Purchase(EShopCurrency currencyType, int cost, CancellationToken token);
        
        bool GrandItem(CoreItemData item);
    }

    public interface IInGamePurchase
    {
        Task<EPurchaseResult> PurchaseInGame(EShopCurrency currencyType, int cost, CancellationToken token);
        Task<EPurchaseResult> PurchaseForAds(CancellationToken token);
        Task<EPurchaseResult> PurchaseInApp(string skuId, CancellationToken token);
        
    }
    
    
}