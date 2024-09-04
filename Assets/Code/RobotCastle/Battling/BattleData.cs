namespace RobotCastle.Battling
{
    [System.Serializable]
    public class BattleData
    {
        public int troopSize = 3;
        public int stageIndex = 0;
        public int playerHealthPoints = 3;

        public static BattleData GetDefault()
        {
            return new BattleData()
            {
                troopSize = 3,
                stageIndex = 0,
                playerHealthPoints = 3,
            };
        }

        public string GetAsStr()
        {
            return $"TroopSize: {troopSize}. Stage {stageIndex}. PlayerHealth: {playerHealthPoints}";
        }
    }
}