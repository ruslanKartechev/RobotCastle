using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.Saving;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(5)]
    public class HeroesAndUnitsFactory : MonoBehaviour, IHeroesAndUnitsFactory
    {
        public bool SpawnHeroOrItem(SpawnMergeItemArgs args, out IItemView itemView)
        {
            var manager = ServiceLocator.Get<MergeManager>();
            var didSpawn = false;
            if (args.usePreferred)
                didSpawn = manager.SpawnOnMergeGrid(args.coreData, args.preferredCoordinated, out itemView);
            else
                didSpawn = manager.SpawnOnMergeGrid(args.coreData, out itemView);
            if (didSpawn)
            {
                if (args.coreData.type is MergeConstants.TypeUnits)
                {
                    var hero = itemView.Transform.GetComponent<HeroController>();
                    var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(args.coreData.id);
                    heroSave.isUnlocked = true;
                    hero.InitComponents(args.coreData.id, heroSave.level, args.coreData.level);
                }
                return true;
            }
            itemView = null;
            CLog.Log($"[BattleGridSpawner] No available cell!");
            var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            ui.ShowNotEnoughSpace();
            return false;
        }

        public bool SpawnHeroWithItems(SpawnMergeItemArgs args, List<CoreItemData> items, out IItemView itemView)
        {
            if (args.coreData.type != MergeConstants.TypeUnits)
            {
                CLog.LogError($"[SpawnHeroWithItems] Not a hero!!. {args.coreData.AsStr()}");
                itemView = null;
                return false;
            }
            var manager = ServiceLocator.Get<MergeManager>();
            var didSpawn = false;
            if (args.usePreferred)
                didSpawn = manager.SpawnOnMergeGrid(args.coreData, args.preferredCoordinated, out itemView);
            else
                didSpawn = manager.SpawnOnMergeGrid(args.coreData, out itemView);
            if (didSpawn)
            {
                var hero = itemView.Transform.GetComponent<HeroController>();
                var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(args.coreData.id);
                heroSave.isUnlocked = true;
                hero.InitComponents(args.coreData.id, heroSave.level, args.coreData.level);
                var mergedItems = MergeFunctions.TryMergeAll(items, MergeConstants.MaxItemLevel);
                var itemsContainer = itemView.Transform.GetComponent<IUnitsItemsContainer>();
                itemsContainer.SetItems(mergedItems);
                return true;
            }
            itemView = null;
            CLog.Log($"[BattleGridSpawner] No available cell!");
            var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            ui.ShowNotEnoughSpace();
            return false;
        }

        private void OnEnable()
        {
            ServiceLocator.Bind<IHeroesAndUnitsFactory>(this);
            ServiceLocator.Bind<HeroesAndUnitsFactory>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<IHeroesAndUnitsFactory>();
            ServiceLocator.Unbind<HeroesAndUnitsFactory>();
        }

    }

}