﻿using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.Saving;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(5)]
    public class BattleGridSpawner : MonoBehaviour, IBattleGridSpawner
    {
        public bool SpawnRandomHero(WeightedList<CoreItemData> ids)
        {
            var coreData = ids.Random();
            return SpawnHero(coreData, out var item);
        }

        public bool SpawnHero(CoreItemData coreData, out IItemView itemView)
        {
            var manager = ServiceLocator.Get<MergeManager>();
            if (manager.SpawnOnMergeGrid(coreData, out itemView))
            {
                if (coreData.type is MergeConstants.TypeUnits)
                {
                    var hero = itemView.Transform.GetComponent<HeroController>();
                    var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(coreData.id);
                    heroSave.isUnlocked = true;
                    hero.InitAsPlayerHero(coreData.id, heroSave.level, coreData.level);
                }
                return true;
            }
            itemView = null;
            CLog.Log($"[BattleGridSpawner] No available cell!");
            var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            ui.ShowNotEnoughSpace();
            return false;
        }

        public bool SpawnHeroWithItems(CoreItemData coreData, List<CoreItemData> items, out IItemView itemView)
        {
            var manager = ServiceLocator.Get<MergeManager>();
            if (manager.SpawnOnMergeGrid(coreData, out itemView))
            {
                if (coreData.type is MergeConstants.TypeUnits)
                {
                    var hero = itemView.Transform.GetComponent<HeroController>();
                    var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(coreData.id);
                    heroSave.isUnlocked = true;
                    hero.InitAsPlayerHero(coreData.id, heroSave.level, coreData.level);
                    var mergedItems = MergeFunctions.TryMergeAll(items, MergeConstants.MaxItemLevel);
                    var itemsContainer = itemView.Transform.GetComponent<IUnitsItemsContainer>();
                    itemsContainer.SetItems(mergedItems);
                }
                else
                {
                    CLog.Log("Not a hero!");
                }
                return true;
            }
            itemView = null;
            CLog.Log($"[BattleGridSpawner] No available cell!");
            var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            ui.ShowNotEnoughSpace();
            return false;
        }

        public bool SpawnRandomHero(WeightedList<string> ids)
        {
            var id = ids.Random();
            return SpawnHero(id);
        }

        public bool SpawnHero(string id)
        {
            var manager = ServiceLocator.Get<MergeManager>();
            if (manager.SpawnOnMergeGrid(id))
                return true;
            CLog.Log($"[BattleGridSpawner] No available cell!");
            var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            ui.ShowNotEnoughSpace();
            return false;
        }
        
        private void OnEnable()
        {
            ServiceLocator.Bind<IBattleGridSpawner>(this);
            ServiceLocator.Bind<BattleGridSpawner>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<IBattleGridSpawner>();
            ServiceLocator.Unbind<BattleGridSpawner>();
        }

    }

}