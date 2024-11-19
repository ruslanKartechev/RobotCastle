using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class TabOther : ShopTab
    {
        [SerializeField] private BlackoutFadeScreen _blackoutFade;
        [SerializeField] private List<ShopItemUI> _items;


        public override void Show()
        {
            gameObject.SetActive(true);
            _blackoutFade.FadeIn();
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}