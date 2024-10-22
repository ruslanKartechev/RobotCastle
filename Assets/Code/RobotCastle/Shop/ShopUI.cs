using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Shop
{
    
    public class ShopUI : MonoBehaviour, IScreenUI
    {
        [SerializeField] private BlackoutFadeScreen _fadeScreen;
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private ShopTabsSwitcher _tabsSwitcher;
        private ShopManager _shopManager = new ();

        public void Show(int firstTab = 1)
        {
            gameObject.SetActive(true);
            _fadeScreen.FadeInWithId(UIConstants.UIShop);
            _raycaster.enabled = true;
            _tabsSwitcher.InitFirstTab(firstTab);
        }

        public void Hide()
        {
            _raycaster.enabled = false;
            _fadeScreen.FadeOut();
        }
        
        private void OnEnable()
        {
            ServiceLocator.Bind<ShopManager>(_shopManager);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<ShopManager>();
        }
    }


    public enum EShopCurrency { Money, HardMoney, REAL_Money, ForAds }

    [System.Serializable]
    public class ShopItemData
    {
        public int cost;
        public EShopCurrency currency; 
        public CoreItemData itemData;
        [Space(10)]
        public bool isAvailable;
        public bool isTimed;
    }

    
    [System.Serializable]
    public class ShopSaveData
    {
        public DateTimeData dailyOfferStartTime;
    }

    
    [System.Serializable]
    public class ShopDataBase
    {
        public List<ShopItemData> dailyOffers;
        public List<ShopItemData> resources;
        public List<ShopItemData> otherOffers;
        
    }
}