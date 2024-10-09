using RobotCastle.Data;

namespace RobotCastle.Battling.DevilsOffer
{
    public enum EDevilsPenaltyType { CastleDurability, AdditionalEnemyForces, HigherEnemyTier }
    
    [System.Serializable]
    public class DevilsOfferData
    {
        public EDevilsPenaltyType penaltyType;
        public float penaltyValue;
        public CoreItemData reward;
        
        public DevilsOfferData(){}

        public DevilsOfferData(DevilsOfferData other)
        {
            penaltyType = other.penaltyType;
            penaltyValue = other.penaltyValue;
            reward = new CoreItemData(other.reward);
        }
    }
}