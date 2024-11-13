namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class RoundData
    {
        public string enemyPreset;
        public int reward;
        public RoundType roundType;
        
        public RoundData(){}

        public RoundData(RoundData other)
        {
            enemyPreset = other.enemyPreset;
            reward = other.reward;
            roundType = other.roundType;
        }

    }
}