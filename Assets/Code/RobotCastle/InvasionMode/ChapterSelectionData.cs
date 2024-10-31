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
        public float basicRewardMultiplier = 1;
        public int totalEnergyCost = BasicEnergyCost;
        public int chapterIndex;
        public int tierIndex;
        public bool corruption;
        
        public ChapterSelectionData(){}

        public ChapterSelectionData(ChapterSelectionData other)
        {
            multiplierTier = other.multiplierTier;
            basicRewardMultiplier = other.basicRewardMultiplier;
            totalEnergyCost = other.totalEnergyCost;
            chapterIndex = other.chapterIndex;
            tierIndex = other.tierIndex;
            corruption = other.corruption;
        }
    }
}