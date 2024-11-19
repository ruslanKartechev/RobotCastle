using System.Collections.Generic;
using MadPixel.InApps;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

namespace RobotCastle.Shop
{
    public class NonConsumableShopItem : ShopItemUI
    {
        [SerializeField] private string _skuId;
        [SerializeField] private List<CoreItemData> _rewards;
        [SerializeField] private FadeInOutAnimator _successAnimator;
        [SerializeField] private FadeInOutAnimator _failedAnimator;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private List<GameObject> _disabledGos;
        private bool _canProcess = true;

        private void OnEnable()
        {
            _btn.AddMainCallback(TryPurchase);
            SetCostText();
        }

        private void SetAvailable(bool available)
        {
            _btn.SetInteractable(available);
            foreach (var go in _disabledGos)
                go.SetActive(!available);
        }

        private void SetCostText()
        {
            try
            {
                var sku = ServiceLocator.Get<InAppsSKUDataBase>().GetSKUWithID(_skuId);
                var product = MobileInAppPurchaser.Instance.GetProduct(sku);
                if (product != default && product.metadata != default)
                {
                    SetAvailable(product.availableToPurchase && !product.hasReceipt);
                    _costText.text = product.metadata.localizedPriceString;
                }
                else
                    CLog.LogError($"Product for sku: {sku} is null!");
            }
            catch (System.Exception ex)
            {
                CLog.LogError(ex.Message, ex.StackTrace);
            }
        }
        
        private void TryPurchase()
        {
            if (!_canProcess) return;
            CLog.Log($"[{gameObject.name}] Trying to purchase");
            try
            {
                var sku = ServiceLocator.Get<InAppsSKUDataBase>().GetSKUWithID(_skuId);
                if (!MobileInAppPurchaser.Exist)
                {
                    CLog.LogRed($"Purchaser does not exists!");
                    _failedAnimator.OnAndFadeOut();
                    return;
                }

                _canProcess = false;
                MobileInAppPurchaser.Instance.OnPurchaseResult -= OnPurchase;
                MobileInAppPurchaser.Instance.OnPurchaseResult += OnPurchase;
                MobileInAppPurchaser.Instance.BuyProductInner(sku);

            }
            catch (System.Exception ex)
            {
                _failedAnimator.OnAndFadeOut();
                CLog.LogError(ex.Message, ex.StackTrace);
            }
        }

        private void OnPurchase(Product product)
        {
            _canProcess = true;
            MobileInAppPurchaser.Instance.OnPurchaseResult -= OnPurchase;
            if(product == default || product.metadata == default)
            {
                CLog.LogError($"Product is null. Failed to purchase");
                _failedAnimator.OnAndFadeOut();
                return;
            }

            var shopManager = ServiceLocator.Get<IShopManager>();
            foreach (var rew in _rewards)
                shopManager.GrandItem(rew);
            _successAnimator.OnAndFadeOut();
            SetAvailable(false);
        }
    }
}