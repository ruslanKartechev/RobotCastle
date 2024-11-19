using System;
using System.Collections.Generic;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class TabResources : ShopTab
    {
        [SerializeField] private BlackoutFadeScreen _blackoutFade;
        [SerializeField] private List<ShopItemInApp> _itemsUI;
        private bool _shallUpdate = true;


        public override void Show()
        {
            gameObject.SetActive(true);
            _blackoutFade.FadeIn();
            if (_shallUpdate)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}