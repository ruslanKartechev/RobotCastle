using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class TabDailyOffers : ShopTab
    {
        private static bool DidInitFirstTime;

        [SerializeField] private int _heroesCount;
        [SerializeField] private int _givenHeroXP = 10;
        [SerializeField] private BlackoutFadeScreen _blackoutFade;
        [SerializeField] private List<DailyShopItemUI> _itemsUI;
        [SerializeField] private List<CoreItemData> _randomOptions;
        [SerializeField] private ShopTimer _timer;
        
        public override void Show()
        {
            gameObject.SetActive(true);
            _blackoutFade.FadeIn();
            if (!DidInitFirstTime)
            {
                DidInitFirstTime = true;
                GenerateNew();
                return;
            }
            Refresh();
        }
        
        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnTimeEnd()
        {
            GenerateNew();
            CLog.Log($"Timer end!");
        }
        
        private void Refresh()
        {
            var save = ServiceLocator.Get<IDataSaver>().GetData<ShopSaveData>();
            var count = _itemsUI.Count;
            if (save.dailyItems.Count == count)
            {
                var shopManager = ServiceLocator.Get<IShopManager>();
                for (var i = 0; i < count; i++)
                {
                    var ui = _itemsUI[i];
                    ui.itemSave = save.dailyItems[i];
                    ui.SetData(save.dailyItems[i].itemData);
                }
                SetTimer(save);
            }
            else
            {
                GenerateNew();    
            }
        }

        private void GenerateNew()
        {
            var count = _itemsUI.Count;
            var saveData = ServiceLocator.Get<IDataSaver>().GetData<ShopSaveData>();
            var savesList = new List<ShopItemSave>();
            var options = new List<CoreItemData>(_randomOptions);
            var shopManager = ServiceLocator.Get<IShopManager>();
            var uiInd = 0;
            for (uiInd = 0; uiInd < count - _heroesCount; uiInd++)
            {
                var item = options.RemoveRandom();
                var save = new ShopItemSave()
                {
                    isAvailable = true,
                    itemData = item
                };
                savesList.Add(save);
                var ui = _itemsUI[uiInd];
                ui.itemSave = save;
                ui.SetData(item);
            }

            var heroes = DataHelpers.GetHeroesSave();
            var unlocked = heroes.heroSaves.FindAll(t => t.isUnlocked);
            if (unlocked.Count < _heroesCount)
            {
                var diff = _heroesCount - unlocked.Count;
                for (var k = 0; k < diff; k++)
                    unlocked.Add(new HeroSave(unlocked[k]));   
            }
            
            for (var i = 0; i < _heroesCount && uiInd < count; i++)
            {
                var item = new CoreItemData
                {
                    id = unlocked.RemoveRandom().id,
                    level = _givenHeroXP,
                    type = ItemsIds.TypeHeroes
                };

                var save = new ShopItemSave()
                {
                    isAvailable = true,
                    itemData = item
                };
                savesList.Add(save);
                var ui = _itemsUI[uiInd];
                ui.itemSave = save;
                ui.SetData(item);
                uiInd++;
            }
            
            saveData.dailyItems = savesList;
            saveData.dailyOfferStartTime = new DateTimeData(DateTime.Now + TimeSpan.FromDays(1));
            SetTimer(saveData);
        }

        private void SetTimer(ShopSaveData saveData)
        {
            _timer.EndTime = saveData.dailyOfferStartTime;
            _timer.TimerEndCallback = OnTimeEnd;
            _timer.Begin();
        }

   
        
    }
}