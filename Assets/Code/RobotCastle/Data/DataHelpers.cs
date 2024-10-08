using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.InvasionMode;
using RobotCastle.Merging;
using RobotCastle.Saving;
using UnityEngine;

namespace RobotCastle.Data
{
    public static class DataHelpers
    {

        public static void SeparateOwnedAndNotOwnedHeroes(SavePlayerHeroes heroes, 
            out List<HeroSave> ownedHeroes, out List<HeroSave> notOwnedHeroes)
        {
            ownedHeroes = new List<HeroSave>(heroes.heroSaves.Count);
            notOwnedHeroes = new List<HeroSave>(heroes.heroSaves.Count);
            foreach (var save in heroes.heroSaves)
            {
                if(save.isUnlocked) ownedHeroes.Add(save);
                else notOwnedHeroes.Add(save);
            }
        }
        
        public static DescriptionInfo GetItemDescription(CoreItemData itemData)
        {
            var id = itemData.id;
            var fullId = $"{id}_lvl_{itemData.level}";
            return ServiceLocator.Get<DescriptionsDataBase>().GetDescription(fullId);
        }

        public static Sprite GetItemIcon(CoreItemData itemData)
        {
            return ServiceLocator.Get<ViewDataBase>().GetUnitItemSpriteAtLevel(itemData.id, itemData.level);
        }

        public static SaveInvasionProgression GetInvasionProgress()
        {
            return ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().progression;
        }

        public static SavePlayerParty GetPlayerParty()
        {
            return ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().party;
        }

        public static SavePlayerData GetPlayerData()
        {
            return ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>();
        }
        
        public static PlayerInventory GetInventory()
        {
            return ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
        }
    }
}