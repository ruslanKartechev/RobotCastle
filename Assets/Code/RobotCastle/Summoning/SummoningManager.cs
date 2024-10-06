using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Summoning
{
    public class SummoningManager
    {
        public static void SetOffTimerAndAddOpenedScrollCount(string id)
        {
            var inv = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
            inv.scrollsMap[id].timerData = DateTimeData.FromNow();
            inv.scrollsMap[id].purchasedCount++;
        }

        public static void AddOpenedScrollCount(string id)
        {
            var inv = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
            inv.scrollsMap[id].purchasedCount++;
        }

        public static void SetOffTimer(string id)
        {
            var inv = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
            inv.scrollsMap[id].timerData = DateTimeData.FromNow();
        }
        

        public static bool CanTakeForFreeOrForAd(string id)
        {
            var db = ServiceLocator.Get<SummoningDataBase>();
            var data = db.GetConfig(id);
            if(!data.freeAvailable && !data.adAvailable) return false;
            var inv = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
            var save = inv.scrollsMap[id];
            var timePassed = save.timerData.CheckIfTimePassed(TimeSpan.FromHours(data.timePeriodHours));
            CLog.LogRed($" ==== {id} Time Passed: {timePassed}");
            return timePassed;
        }

        public static bool CanTakeForFree(string id)
        {
            var db = ServiceLocator.Get<SummoningDataBase>();
            var data = db.GetConfig(id);
            if(!data.freeAvailable) return false;
            var inv = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
            var save = inv.scrollsMap[id];
            var timePassed = save.timerData.CheckIfTimePassed(TimeSpan.FromHours(data.timePeriodHours));
            CLog.LogRed($" ==== Time Passed: {timePassed}");
            return timePassed;
        }
        
        public static bool CanTakeForAds(string id)
        {
            var db = ServiceLocator.Get<SummoningDataBase>();
            var data = db.GetConfig(id);
            if(!data.adAvailable) return false;
            var inv = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
            var save = inv.scrollsMap[id];
            var timePassed = save.timerData.CheckIfTimePassed(TimeSpan.FromHours(data.timePeriodHours));
            CLog.LogRed($" ==== Time Passed: {timePassed}");
            return timePassed;
        }
        
        /// <summary>
        /// Will use and subtract scrolls from inventory OR spend currency instead to randomly pick output items using SummonConfig for the given scroll ID
        /// </summary>
        public static bool TryGetOutputFromScroll(string scrollId, int scrollsCount, float multiplier, out List<SummonOutput> output)
        {
            var inv = DataHelpers.GetInventory();
            var scrolls = inv.GetScrollsCount(scrollId);
            CLog.Log($"[{nameof(TryGetOutputFromScroll)}] id: {scrollId}, owned: {scrolls}, required: {scrollsCount}, multiplier: {multiplier}");
            if (scrolls >= scrollsCount)
            {
                scrolls -= scrollsCount;
                inv.SetScrollsCount(scrollId, scrolls);
                output = GetRandomItemsAndApply(scrollId, multiplier);
                return true;
            }

            var db = ServiceLocator.Get<SummoningDataBase>();
            var config = db.GetConfig(scrollId);
            var money = GetMoneyAmount(config.currencyId);
            var cost = Mathf.RoundToInt(config.purchaseCost * multiplier);
            if (money < cost)
            {
                CLog.Log($"[{nameof(TryGetOutputFromScroll)}] NOT enough money to purchase: x{scrollsCount} of {scrollId}");
                output = null;
                return false;
            }
            CLog.Log($"[{nameof(TryGetOutputFromScroll)}] Enough money to purchase: x{scrollsCount} of {scrollId}");
            money -= cost;
            SetMoneyAmount(config.currencyId, money);
            output = GetRandomItemsAndApply(scrollId, multiplier);
            return true;
        }

        public static void SetHeroAsFirst(List<SummonOutput> output)
        {
            var targetInd = -1;
            for (var i = 0; i < output.Count; i++)
            {
                var res = output[i];
                if (res.data.type == SummonConfig.Id_NewHero)
                {
                    targetInd = i;
                    break;
                }
            }
            if (targetInd == -1)
                return;
            (output[0], output[targetInd]) = (output[targetInd], output[0]);
        }
        
        public static bool CheckEnoughMoney(string scrollId)
        {
            var db = ServiceLocator.Get<SummoningDataBase>();
            var config = db.GetConfig(scrollId);
            var cost = config.purchaseCost;
            return GetMoneyAmount(config.currencyId) >= cost;
        }

        public static void SetMoneyAmount(string currencyId, int amount)
        {
            switch (currencyId)
            {
                case ItemsIds.IdMoney:
                    ServiceLocator.Get<GameMoney>().globalMoney = amount;
                    break;
                case ItemsIds.IdHardMoney:
                    ServiceLocator.Get<GameMoney>().globalHardMoney = amount;
                    break;
                case ItemsIds.IdKingMedal or ItemsIds.IdHeroMedal:
                    DataHelpers.GetInventory().SetCount(currencyId, amount);
                    break;
                default:
                    CLog.LogError($"Cannot process currency with id: {currencyId}");
                    break;
            }
        }

        public static int GetMoneyAmount(string currencyId)
        {
            var money = 0;
            switch (currencyId)
            {
                case ItemsIds.IdMoney:
                    money = ServiceLocator.Get<GameMoney>().globalMoney;
                    break;
                case ItemsIds.IdHardMoney:
                    money = ServiceLocator.Get<GameMoney>().globalHardMoney;
                    break;
                case ItemsIds.IdKingMedal or ItemsIds.IdHeroMedal:
                    money = DataHelpers.GetInventory().GetCount(currencyId);
                    break;
                default:
                    CLog.LogError($"Cannot process currency with id: {currencyId}");
                    break;
            }
            return money;
        }        
        
        /// <summary>
        /// Will try to find config file for the given id and then randomly pick output items SummonConfig from that file
        /// All outputs will be added to player money, heroes pool or inventory immediately
        /// </summary>
        public static List<SummonOutput> GetRandomItemsAndApply(string summonId, float outputMultiplier = 1)
        {
            var config = GetConfigForId(summonId);
            if (config == null)
                return null;
            return GetRandomItemsAndApply(config, outputMultiplier);
        }
        
        /// <summary>
        /// Will randomly pick output results with the given summon config.
        /// All outputs will be added to player money, heroes pool or inventory immediately
        /// </summary>
        public static List<SummonOutput> GetRandomItemsAndApply(SummonConfig config, float outputMultiplier = 1)
        {
            var outputResult = new List<SummonOutput>(config.outputItemsCount);
            var saver = ServiceLocator.Get<IDataSaver>();
            var heroes = saver.GetData<SavePlayerHeroes>();
            var inventory = saver.GetData<SavePlayerData>().inventory;
            var totalCount = Mathf.CeilToInt(outputMultiplier * config.outputItemsCount);
            var didGetNewHero = false;
            var newHeroOption = config.options.Find(t => t.id == SummonConfig.Id_NewHero);
            if (newHeroOption == null)
                CLog.Log($"newHeroOption is not preset in the config");
            for (var i = 0; i < totalCount; i++)
            {
                var option = GetRandomOption(config, didGetNewHero ? newHeroOption : null);
                if (option == null)
                {
                    CLog.LogError($"Error in random option calculation. Obtained null option");
                    return null;
                }
                CLog.Log($"Step {i}, option id: {option.id}");
                var id = option.id;
                DataHelpers.SeparateOwnedAndNotOwnedHeroes(heroes, out var ownedHeroes, out var notOwnedHeroes);
                SummonOutput existing;
                switch (id)
                {
                    case SummonConfig.Id_OwnedHero:
                        // CLog.Log($"Added already owned hero");
                        var xpHero = ownedHeroes.Random();
                        xpHero.xp += UnityEngine.Random.Range(option.amount, option.amountMax);
                        existing = outputResult.Find(t => t != null && t.data.id == xpHero.id && t.data.type == SummonConfig.Id_OwnedHero );
                        if (existing != null)
                            existing.data.level += xpHero.xp;
                        else
                            outputResult.Add(new SummonOutput(SummonConfig.Id_OwnedHero, xpHero.id, xpHero.xp));
                        break;
                    case SummonConfig.Id_NewHero:
                        if(notOwnedHeroes.Count > 0)
                        {
                            var newHero = notOwnedHeroes.Random();
                            CLog.Log($"Added new hero: {newHero.id}");
                            newHero.isUnlocked = true;
                            notOwnedHeroes.Remove(newHero);
                            ownedHeroes.Add(newHero);
                            outputResult.Add(new SummonOutput(SummonConfig.Id_NewHero, newHero.id, 1));
                        }
                        else
                        {
                            CLog.Log("No locked heroes, all heroes are owned");
                            AddItem(ItemsIds.IdHeroMedal, SummonConfig.HeroMedalsIfAllHeroesOwned);
                        }
                        didGetNewHero = true;
                        break;
                    case SummonConfig.Id_Gold:
                        var amount = UnityEngine.Random.Range(option.amount, option.amountMax);
                        existing = outputResult.Find(t => t != null && t.data.id == SummonConfig.Id_Gold && t.data.type == SummonConfig.Id_Gold );
                        if (existing != null)
                            existing.data.level += amount;
                        else
                            outputResult.Add(new SummonOutput(SummonConfig.Id_Gold, SummonConfig.Id_Gold, amount));
                        break;
                    default:
                        AddItem(option.id, UnityEngine.Random.Range(option.amount, option.amountMax));
                        break;
                }

                void AddItem(string itemId, int amount)
                {
                    // CLog.Log($"Adding inventory item: {itemId}, count: {option.amount}");
                    existing = outputResult.Find(t => t != null && t.data.id == itemId && t.data.type == SummonConfig.Id_AnyInventoryItem );
                    if (existing != null)
                        existing.data.level += amount;
                    else
                        outputResult.Add(new SummonOutput(SummonConfig.Id_AnyInventoryItem, itemId, amount));
                        
                    if (inventory.itemsMap.ContainsKey(id) == false)
                        inventory.itemsMap.Add(id, new InventoryItemData(id, amount));
                    else
                        inventory.itemsMap[id].amount += amount;
                }
            }

            var mon = outputResult.Find(t => t.data.type == SummonConfig.Id_Gold);
            if (mon != null)
            {
                CLog.Log($"Adding gold: {mon.data.level}");
                ServiceLocator.Get<GameMoney>().AddGlobalMoney(mon.data.level);
            }
            return outputResult;
        }
      

        private static SummonConfig.Option GetRandomOption(SummonConfig config, SummonConfig.Option excluded = null)
        {
            var list = config.options;
            if (excluded != null)
            {
                list = new List<SummonConfig.Option>(config.options);
                list.Remove(excluded);
            }
            var result = (SummonConfig.Option)null;
            var total = 0f;
            foreach (var op in list)
                total += op.chance;
            var randomVal = UnityEngine.Random.Range(0f, total);
            CLog.Log($"Picking from: {list.Count}. Total: {total}. Random: {randomVal:N2}");
            foreach (var op in list)
            {
                randomVal -= op.chance;
                if (randomVal <= 0)
                {
                    result = op;
                    break;
                }
            }
            return result;
        }


        private static SummonConfig GetConfigForId(string summonId)
        {
            var path = $"summoning/{summonId}";
            var asset = Resources.Load<TextAsset>(path);
            if (asset == null)
            {
                CLog.LogError($"Error loading file: {path}");
                return null;
            }
            var res = JsonConvert.DeserializeObject<SummonConfig>(asset.text);
            if (res == null)
            {
                CLog.LogRed($"Error deserializing {path} to [{nameof(SummonConfig)}]");
            }
            return res;
        }
    }
}