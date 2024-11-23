using SleepDev;

namespace RobotCastle.InvasionMode
{
    [System.Serializable]
    public class ChapterSelectionData
    {
        public const int BasicEnergyCost = 5;
        public const int EnergyCostPerMultiplier = 5;
        /// <summary>
        /// Including
        /// </summary>
        public const int MaxMultiplierTier = 3; 
        public const int MinMultiplierTier = 1; 

        /// <summary>
        /// either 1,2 or 3
        /// </summary>
        public int multiplierTier = 1;
        public float tierRewardMultiplier = 1;
        public int totalEnergyCost = BasicEnergyCost;
        public int chapterIndex;
        public int tierIndex;
        public bool corruption;
        public int goldReward;
        /// <summary>
        /// 0 => regular, 1 => corruption
        /// </summary>
        public int mode = 0;

        public bool IsCorruption => mode == 1;
        public bool IsNormal => mode == 0;

        public string GetModeAsStr()
        {
            switch (mode)
            {
                case 0: return "normal";
                case 1: return "corruption";
                default:
                    CLog.LogError($"GameMode: {mode} is unknown code!!");
                return "unknown";
            }
        }
        
        public ChapterSelectionData(){}

        public ChapterSelectionData(ChapterSelectionData other)
        {
            multiplierTier = other.multiplierTier;
            tierRewardMultiplier = other.tierRewardMultiplier;
            totalEnergyCost = other.totalEnergyCost;
            chapterIndex = other.chapterIndex;
            tierIndex = other.tierIndex;
            corruption = other.corruption;
            goldReward = other.goldReward;
        }
    }
}