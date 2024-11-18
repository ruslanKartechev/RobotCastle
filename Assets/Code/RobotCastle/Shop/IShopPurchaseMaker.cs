using System;

namespace RobotCastle.Shop
{
    public interface IShopPurchaseMaker
    {
        void TryPurchase(Action<EPurchaseResult> callback);
    }
}