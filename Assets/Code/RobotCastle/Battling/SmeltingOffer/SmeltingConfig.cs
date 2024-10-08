using System.Collections.Generic;

namespace RobotCastle.Battling.SmeltingOffer
{
    [System.Serializable]
    public class SmeltingConfig
    {
        /// <summary>
        /// Smelting data for every Offer's Tier
        /// </summary>
        public List<SmeltingData> smeltingTiers;
    }
}