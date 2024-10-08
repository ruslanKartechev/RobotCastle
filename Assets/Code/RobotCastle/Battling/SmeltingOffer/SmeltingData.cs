using System.Collections.Generic;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Battling.SmeltingOffer
{
    [System.Serializable]
    public class SmeltingData
    {
        public List<CoreItemData> itemsOptions;
        
    }
    
    [System.Serializable]
    public class SmeltingConfig
    {
        public List<SmeltingData> smeltingTiers;
    }
}