namespace RobotCastle.Core
{
    [System.Serializable]
    public class SavePlayerData
    {
        public int money;
        public int globalMoney;
        public int globalGems;
        public int playerLevel;
        
        public SavePlayerData(){}

        public SavePlayerData(SavePlayerData other)
        {
            money = other.money;
            playerLevel = other.playerLevel;
            globalGems = other.globalGems;
            globalMoney = other.globalMoney;
        }
    }
}