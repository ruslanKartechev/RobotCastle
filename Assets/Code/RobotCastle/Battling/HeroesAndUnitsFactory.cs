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
    // All spawning of items and units happens here.
    // Supply CoreItemData, additional items (optionally), target cell or grid and grid sections controller
    [DefaultExecutionOrder(5)]
    public class HeroesAndUnitsFactory : MonoBehaviour, IHeroesAndUnitsFactory
    {
        public bool SpawnHeroOrItem(SpawnMergeItemArgs args, 
            IGridView grid, 
            IGridSectionsController sectionsController, 
            out IItemView spawnedItem)
        {
            Vector2Int coord = default;
            if (args.usePreferredCoordinate)
            {
                coord = args.preferredCoordinated;
            }
            else
            {
                var hasFree = sectionsController.GetFreeCell(grid.BuiltGrid, out coord);
                if (!hasFree)
                {
                    CLog.Log($"[{nameof(HeroesAndUnitsFactory)}] No available cell!");

                    var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
                    ui.ShowNotEnoughSpace();
                    spawnedItem = null;
                    return false;
                }
            }
            var cell = grid.GetCell(coord.x, coord.y);
            args.ItemData = new ItemData(args.coreData);
            if (sectionsController.IsCellAllowed(coord.x, coord.y, args.ItemData))
            {
                SpawnHeroOrItem(args, cell, out spawnedItem);
                sectionsController.OnItemPut(spawnedItem.itemData);
                return true;
            }
            spawnedItem = null;
            return false;
        }        
        
        public void SpawnHeroOrItem(SpawnMergeItemArgs args, ICellView cellView, out IItemView spawnedItem)
        {
            var spawner = ServiceLocator.Get<IMergeItemsFactory>();
            spawnedItem = spawner.SpawnItemOnCell(cellView, args.ItemData);
            if (args.coreData.type is MergeConstants.TypeUnits)
            {
                var hero = spawnedItem.Transform.GetComponent<HeroController>();
                var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(args.coreData.id);
                heroSave.isUnlocked = true;
                hero.InitComponents(args.coreData.id, heroSave.level, args.coreData.level);
                if (args.useAdditionalItems)
                {
                    var mergedItems = MergeFunctions.TryMergeAll(args.additionalItems, MergeConstants.MaxItemLevel);
                    var itemsContainer = hero.gameObject.GetComponent<IUnitsItemsContainer>();
                    itemsContainer.SetItems(mergedItems);
                }
            }
        }        
        
        // public bool SpawnHeroOrItem(SpawnMergeItemArgs args, out IItemView itemView)
        // {
        //     var manager = ServiceLocator.Get<MergeManager>();
        //     var didSpawn = false;
        //     if (args.usePreferredCoordinate)
        //         didSpawn = manager.SpawnOnMergeGrid(args.coreData, args.preferredCoordinated, out itemView);
        //     else
        //         didSpawn = manager.SpawnOnMergeGrid(args.coreData, out itemView);
        //     if (didSpawn)
        //     {
        //         if (args.coreData.type is MergeConstants.TypeUnits)
        //         {
        //             var hero = itemView.Transform.GetComponent<HeroController>();
        //             var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(args.coreData.id);
        //             heroSave.isUnlocked = true;
        //             hero.InitComponents(args.coreData.id, heroSave.level, args.coreData.level);
        //         }
        //         return true;
        //     }
        //     itemView = null;
        //     CLog.Log($"[BattleGridSpawner] No available cell!");
        //     var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
        //     ui.ShowNotEnoughSpace();
        //     return false;
        // }
        //
        // public bool SpawnHeroWithItems(SpawnMergeItemArgs args, List<CoreItemData> items, out IItemView itemView)
        // {
        //     if (args.coreData.type != MergeConstants.TypeUnits)
        //     {
        //         CLog.LogError($"[SpawnHeroWithItems] Not a hero!!. {args.coreData.AsStr()}");
        //         itemView = null;
        //         return false;
        //     }
        //     var manager = ServiceLocator.Get<MergeManager>();
        //     var didSpawn = false;
        //     if (args.usePreferredCoordinate)
        //         didSpawn = manager.SpawnOnMergeGrid(args.coreData, args.preferredCoordinated, out itemView);
        //     else
        //         didSpawn = manager.SpawnOnMergeGrid(args.coreData, out itemView);
        //     if (didSpawn)
        //     {
        //         var hero = itemView.Transform.GetComponent<HeroController>();
        //         var heroSave = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(args.coreData.id);
        //         heroSave.isUnlocked = true;
        //         hero.InitComponents(args.coreData.id, heroSave.level, args.coreData.level);
        //         var mergedItems = MergeFunctions.TryMergeAll(items, MergeConstants.MaxItemLevel);
        //         var itemsContainer = itemView.Transform.GetComponent<IUnitsItemsContainer>();
        //         itemsContainer.SetItems(mergedItems);
        //         return true;
        //     }
        //     itemView = null;
        //     CLog.Log($"[BattleGridSpawner] No available cell!");
        //     var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
        //     ui.ShowNotEnoughSpace();
        //     return false;
        // }

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