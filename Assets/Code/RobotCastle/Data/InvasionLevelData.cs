using System.Collections.Generic;

namespace RobotCastle.Data
{
    [System.Serializable]
    public class InvasionLevelData
    {
        public string viewName = "Magic Forest";
        public List<InvasionRoundData> levels;
        public int afterLevelReward = 100;
    }

    [System.Serializable]
    public class InvasionRoundData
    {
        public string enemyPreset;
        public int reward;
        public RoundType roundType;

    }
    
    public enum RoundType{Default, Smelting, EliteEnemy, Boss}
}