using System;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class ShopPurchaseAds : MonoBehaviour, IShopPurchaseMaker
    {
        public void TryPurchase(Action<EPurchaseResult> callback)
        {
            var (msg, res) = AdsPlayer.Instance.PlayReward((did) =>
            {
                var result = did ? EPurchaseResult.Success : EPurchaseResult.Error;
                callback?.Invoke(result);
            }, AdsPlayer.Placement_Shop);
        }
    }
}