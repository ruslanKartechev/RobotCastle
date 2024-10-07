﻿using RobotCastle.Data;
using RobotCastle.InvasionMode;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class SavePlayerData
    {
        public int levelMoney;
        public int globalMoney;
        public int globalHardMoney;
        public int playerLevel;
        public int playerXp;
        public int playerEnergy;
        public int playerEnergyMax;
        public EGameMode gameMode;
        public SavePlayerParty party;
        public PlayerInventory inventory;
        public SaveInvasionProgression progression;
        
        public SavePlayerData(){}

        public SavePlayerData(SavePlayerData other)
        {
            levelMoney = other.levelMoney;
            globalMoney = other.globalMoney;
            globalHardMoney = other.globalHardMoney;
            playerLevel = other.playerLevel;
            playerXp = other.playerXp;
            playerEnergy = other.playerEnergy;
            playerEnergyMax = other.playerEnergyMax;
            gameMode = other.gameMode;
            party = new SavePlayerParty(other.party);
            inventory = new PlayerInventory(other.inventory);
            progression = new SaveInvasionProgression(other.progression);
        }
    }
}