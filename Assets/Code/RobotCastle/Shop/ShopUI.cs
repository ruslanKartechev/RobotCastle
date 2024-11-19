using System;
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
            ServiceLocator.Bind<IShopManager>(_shopManager);
            ServiceLocator.Bind<ShopManager>(_shopManager);
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
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<IShopManager>();            
            ServiceLocator.Unbind<ShopManager>();
        }
    }
}