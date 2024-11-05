using RobotCastle.Core;
using RobotCastle.Data;
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
            switch (args.coreData.type)
            {
                case MergeConstants.TypeHeroes:
                    var heroGo = spawnedItem.Transform.gameObject;
                    var hero = heroGo.GetComponent<IHeroController>();
                    var ui = ServiceLocator.Get<IUIManager>().Show<UnitsUIPanel>(UIConstants.UIHeroesBars, () => { });
                    ui.AssignHeroUI(hero.Components);
                    var id = args.coreData.id;
                    // heroSave.isUnlocked = true;
                    var modifiers = HeroesManager.GetModifiersForHero(id);
                    hero.InitHero(id, HeroesManager.GetHeroLevel(id), args.coreData.level, modifiers);
                    var weaponsContainer = heroGo.GetComponent<IHeroWeaponsContainer>();
                    weaponsContainer.SetEmpty();
                    if (args.useAdditionalItems)
                    {
                        var weaponData = ServiceLocator.Get<ModifiersDataBase>()
                            .GetWeaponsWithModifiers(args.additionalItems);
                        var mergedItems = MergeFunctions.TryMergeAll(weaponData, MergeConstants.MaxItemLevel);
                        weaponsContainer.SetItems(mergedItems);
                    }
                    hero.SetBehaviour(new HeroIdleBehaviour());
                    hero.Components.heroUI.UpdateStatsView(hero.Components);
                    break;
                case MergeConstants.TypeWeapons:
                    var mods = ServiceLocator.Get<ModifiersDataBase>().GetWeaponsWithModifiers(args.coreData);
                    var modsContainer = spawnedItem.Transform.gameObject.GetComponent<ModifiersContainer>();
                    modsContainer.OverrideModifiers(mods.modifierIds);
                    break;

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