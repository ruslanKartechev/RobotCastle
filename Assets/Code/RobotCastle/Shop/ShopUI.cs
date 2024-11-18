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


    [System.Serializable]
    public class ShopSaveData
    {
        public DateTimeData dailyOfferStartTime;
        public List<ShopItemSave> dailyItems;
        
        public ShopSaveData(){}

        public ShopSaveData(ShopSaveData other)
        {
            dailyOfferStartTime = new DateTimeData(other.dailyOfferStartTime);
            var count = other.dailyItems.Count;
            dailyItems = new(count);
            for (var i = 0; i < count; i++)
            {
                dailyItems.Add(new ShopItemSave(other.dailyItems[i]));
            }
        }
    }

    
    [System.Serializable]
    public class ShopDataBase
    {
        public List<ShopItemData> dailyOffers;
        public List<ShopItemData> resources;
        public List<ShopItemData> otherOffers;
        
    }
}