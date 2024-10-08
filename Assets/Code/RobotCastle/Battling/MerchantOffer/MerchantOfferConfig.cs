using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.Battling.MerchantOffer
{
    [System.Serializable]
    public class MerchantOfferConfig
    {
        public List<MerchantOfferData> optionsPerTier;
    }

    [System.Serializable]
    public class MerchantOfferData
    {
        public List<GoodsPreset> presets;

        public GoodsPreset GetRandomPreset()
        {
            return presets[0];
        }
        
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