using System.Collections.Generic;
using NSubstitute.Core;
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
    public class HeroesAndItemsFactory : MonoBehaviour, IHeroesAndItemsFactory
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
                    CLog.Log($"[{nameof(HeroesAndItemsFactory)}] No available cell!");

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
                hero.InitHero(args.coreData.id, heroSave.level, args.coreData.level);
                if (args.useAdditionalItems)
                {
                    var mergedItems = MergeFunctions.TryMergeAll(args.additionalItems, MergeConstants.MaxItemLevel);
                    var itemsContainer = hero.gameObject.GetComponent<IUnitsItemsContainer>();
                    itemsContainer.SetItems(mergedItems);
                }
                hero.SetIdle();
                hero.View.heroUI.UpdateStatsView(hero.View);
            }
        }        
        

        private void OnEnable()
        {
            ServiceLocator.Bind<IHeroesAndItemsFactory>(this);
            ServiceLocator.Bind<HeroesAndItemsFactory>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<IHeroesAndItemsFactory>();
            ServiceLocator.Unbind<HeroesAndItemsFactory>();
        }

    }

}