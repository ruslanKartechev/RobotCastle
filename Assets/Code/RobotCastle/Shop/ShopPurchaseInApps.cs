using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MadPixel.InApps;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;
using UnityEngine.Purchasing;

namespace RobotCastle.Shop
{
    public class ShopPurchaseInApps : MonoBehaviour, IShopPurchaseMaker
    {
        [SerializeField] private string _inAppId;
        private Action<EPurchaseResult> _callback;

        public void TryPurchase(Action<EPurchaseResult> callback)
        {
            var skuDb = ServiceLocator.Get<InAppsSKUDataBase>();
            try
            {
                var sku = skuDb.GetSKUWithID(_inAppId);
                _callback = callback;
                MobileInAppPurchaser.Instance.BuyProductInner(sku);
                MobileInAppPurchaser.Instance.OnPurchaseResult -= OnPurchase;
                MobileInAppPurchaser.Instance.OnPurchaseResult += OnPurchase;
                return;
            }
            catch (System.Exception ex)
            {
                CLog.LogException(ex.Message, ex.StackTrace);
                _callback?.Invoke(EPurchaseResult.Error);
            }
        }

        private void OnPurchase(Product product)
        {
            MobileInAppPurchaser.Instance.OnPurchaseResult -= OnPurchase;
            if (product == null)
            {
                CLog.LogError($"Product is null!");
                _callback?.Invoke(EPurchaseResult.Error);
                return;
            }
            _callback?.Invoke(EPurchaseResult.Success);            
        }
        
        
    }
}