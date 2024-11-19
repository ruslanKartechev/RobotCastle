using MadPixel.InApps;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;
using SleepDev.FlyingUI;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

namespace RobotCastle.Shop
{
    public class ShopItemInApp : ShopItemUI
    {
        [SerializeField] private string _skuId;
        [SerializeField] private CoreItemData _reward;
        [SerializeField] private FadeInOutAnimator _successAnimator;
        [SerializeField] private FadeInOutAnimator _failedAnimator;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Sprite _rewardSprite;
        [SerializeField] private int _flyingRewardCount = 5;
        private bool _canProcess = true;

        private void OnEnable()
        {
            _btn.AddMainCallback(TryPurchase);
            SetCostText();
        }

        private void SetCostText()
        {
            try
            {
                var sku = ServiceLocator.Get<InAppsSKUDataBase>().GetSKUWithID(_skuId);
                var product = MobileInAppPurchaser.Instance.GetProduct(sku);
                if(product != default && product.metadata != default)
                    _costText.text = product.metadata.localizedPriceString;
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
            ServiceLocator.Get<IShopManager>().GrandItem(_reward);
            _successAnimator.OnAndFadeOut();
            Vector3 flyPos;
            switch (_reward.id)
            {
                case ItemsIds.IdMoney:
                    var temp = ServiceLocator.Get<IUIManager>().GetIfShown<PlayerDataPanelUI>(UIConstants.UIPlayerData);
                    if (temp != null)
                        flyPos = temp.moneyPoint.position;
                    else
                        goto default;
                    break;
                case ItemsIds.IdHardMoney:
                    var temp2 = ServiceLocator.Get<IUIManager>().GetIfShown<PlayerDataPanelUI>(UIConstants.UIPlayerData);
                    if (temp2 != null)
                        flyPos = temp2.hardMoneyPoint.position;
                    else
                        goto default;
                    break;
                default:
                    flyPos = new Vector3(Screen.width / 2f, Screen.height, 0);
                    break;
            }
            var flyingUI = ServiceLocator.Get<FlyingUIScreen>();
            flyingUI.FlyFromTo(_rewardSprite, transform.position, flyPos, _flyingRewardCount);

        }
    }
}