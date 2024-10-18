using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class TabDailyOffers : ShopTab
    {
        [SerializeField] private BlackoutFadeScreen _blackoutFade;
        [SerializeField] private List<ShopItemUI> _itemsUI;
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
            var db = ServiceLocator.Get<ShopDataBase>();
            var count = _itemsUI.Count;
            var dataCount = db.dailyOffers.Count;
            for (var i = 0; i < count; i++)
            {
                if(i < dataCount)
                    _itemsUI[i].SetupItem(db.dailyOffers[i]);
                else
                {
                    CLog.LogError($"ui index > database list count");
                    break;
                }
            }
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}