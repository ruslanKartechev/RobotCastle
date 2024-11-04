using System.Collections.Generic;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Battling.MerchantOffer
{
    [System.Serializable]
    public class MerchantOfferConfig
    {
        public List<GoodsPreset> optionsPerTier;
        
        
           
        [System.Serializable]
        public class GoodsPreset
        {
            public List<Goods> goods;
        }
        
        [System.Serializable]
        public class Goods
        {
            public int cost;
            public bool forAds;
            public CoreItemData ItemData;
        }
    }

}