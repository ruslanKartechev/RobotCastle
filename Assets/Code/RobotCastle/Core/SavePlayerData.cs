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
        public PlayerScrollsData scrollsData;
        public SavePlayerParty party;
        public PlayerInventory inventory;
        
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
            scrollsData = new PlayerScrollsData(other.scrollsData);
            party = new SavePlayerParty(other.party);
            inventory = new PlayerInventory(other.inventory);
        }
    }
}