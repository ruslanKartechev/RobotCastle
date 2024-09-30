using RobotCastle.Core;
using RobotCastle.Merging;
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
            args.ItemData = new ItemData(args.coreData);
            if (args.usePreferredCoordinate)
            {
                coord = args.preferredCoordinated;
                if (!sectionsController.IsCellAllowed(coord, args.ItemData))
                {
                    CLog.Log($"Cell {coord} is not allowed");
                    spawnedItem = null;
                    return false;
                }
            }
            else
            {
                var hasFree = sectionsController.GetFreeAllowedCell(grid.BuiltGrid, args.ItemData, out coord);
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
            SpawnOnCell(args, cell, out spawnedItem);
            sectionsController.OnItemPut(spawnedItem.itemData);
            return true;
        }        
        
        public void SpawnOnCell(SpawnMergeItemArgs args, ICellView cellView, out IItemView spawnedItem)
        {
            var spawner = ServiceLocator.Get<IMergeItemsFactory>();
            spawnedItem = spawner.SpawnItemOnCell(cellView, args.ItemData);
            if (args.coreData.type is MergeConstants.TypeUnits)
            {
                var heroGo = spawnedItem.Transform.gameObject;
                var hero = heroGo.GetComponent<IHeroController>();
                var barsPanel = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
                barsPanel.AssignHeroUI(hero.View);
                var id = args.coreData.id;
                // heroSave.isUnlocked = true;
                var modifiers = HeroesHelper.GetModifiersForHero(id);
                hero.InitHero(id, HeroesHelper.GetHeroLevel(id), args.coreData.level, modifiers);
                if (args.useAdditionalItems)
                {
                    var mergedItems = MergeFunctions.TryMergeAll(HeroItemData.GetDataWithDefaultModifiers(args.additionalItems), MergeConstants.MaxItemLevel);
                    var itemsContainer = heroGo.GetComponent<IHeroItemsContainer>();
                    itemsContainer.SetItems(mergedItems);
                }
                hero.SetBehaviour(new HeroIdleBehaviour());
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