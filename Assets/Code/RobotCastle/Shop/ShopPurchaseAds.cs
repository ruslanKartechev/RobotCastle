using System;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class ShopPurchaseAds : MonoBehaviour, IShopPurchaseMaker
    {
        [SerializeField] private FadeInOutAnimator _successAnimator;
        [SerializeField] private FadeInOutAnimator _failedAnimator;
        
        public void TryPurchase(Action<EPurchaseResult> callback)
        {
            var (msg, res) = AdsPlayer.Instance.PlayReward((did) =>
            {
                var result = did ? EPurchaseResult.Success : EPurchaseResult.Error;
                callback?.Invoke(result);
                if (did)
                    _successAnimator.OnAndFadeOut();
                else
                    _failedAnimator.OnAndFadeOut();
            }, AdsPlayer.Placement_Shop);
        }
    }
}