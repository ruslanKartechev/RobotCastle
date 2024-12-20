﻿using RobotCastle.Battling.Altars;
using RobotCastle.Data;
using RobotCastle.InvasionMode;
using RobotCastle.Relics;
using SleepDev;

namespace RobotCastle.Core
{
    [System.Serializable]
    public class SavePlayerData
    {
        public bool soundOn;
        public bool musicOn;
        public bool vibrOn;
        public int levelMoney;
        public int globalMoney;
        public int globalHardMoney;
        public int playerLevel;
        public int playerXp;
        public int playerEnergy;
        public int playerEnergyMax;
        public DateTimeData timeWhenOutOfEnergy;
        public EGameMode gameMode;
        public SavePlayerParty party;
        public PlayerInventory inventory;
        public RelicsInventorySave relics;
        public SaveInvasionProgression progression;
        public ChapterSelectionData chapterSelectionData;
        public AltarsSave altars;
        public TutorialSave tutorials;
        
        public SavePlayerData(){}

        public SavePlayerData(SavePlayerData other)
        {
            soundOn = other.soundOn;
            musicOn = other.musicOn;
            vibrOn = other.vibrOn;
            levelMoney = other.levelMoney;
            globalMoney = other.globalMoney;
            globalHardMoney = other.globalHardMoney;
            playerLevel = other.playerLevel;
            playerXp = other.playerXp;
            playerEnergy = other.playerEnergy;
            playerEnergyMax = other.playerEnergyMax;
            timeWhenOutOfEnergy = new DateTimeData(other.timeWhenOutOfEnergy);
            gameMode = other.gameMode;
            party = new SavePlayerParty(other.party);
            inventory = new PlayerInventory(other.inventory);
            progression = new SaveInvasionProgression(other.progression);
            chapterSelectionData = new ChapterSelectionData(other.chapterSelectionData);
            altars = new AltarsSave(other.altars);
            relics = new RelicsInventorySave(other.relics);
            tutorials = new TutorialSave(other.tutorials);
        }

        public void InitAfterLoad()
        {
            inventory.InitAfterLoad();
        }
        
        public void PrepareForSave()
        {
            inventory.PrepareForSave();
        }
    }
}