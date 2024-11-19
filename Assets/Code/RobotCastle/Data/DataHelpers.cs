using System.Collections.Generic;
using RobotCastle.Battling.Altars;
using RobotCastle.Core;
using RobotCastle.InvasionMode;
using RobotCastle.Saving;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Data
{

    public static class DataHelpers
    {
    
        /// <summary>
        /// Will call PrepareForSave() On money and inventory save data before calling IDataSaver.Save()
        /// </summary>
        public static void SaveData()
        {
            var playerData = GetPlayerData();
            playerData.PrepareForSave();
            if (ServiceLocator.GetIfContains<GameMoney>(out var gameMoney))
            {
                gameMoney.PrepareForSave();
            }
            ServiceLocator.Get<IDataSaver>().SaveAll();
            CLog.LogGreen("[DataHelpers] === Data Saved !!");
        }
        
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

        public static Sprite GetItemIcon(CoreItemData itemData)
        {
            return ServiceLocator.Get<ViewDataBase>().GetWeaponSprite(itemData.id, itemData.level);
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
        
        public static SavePlayerHeroes GetHeroesSave()
        {
            return ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>();
        }
        
        public static PlayerInventory GetInventory()
        {
            return ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().inventory;
        }

        public static AltarsSave GetAltarsSave()
        {
            return ServiceLocator.Get<IDataSaver>().GetData<SavePlayerData>().altars;
        }
    }
}