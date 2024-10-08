using UnityEngine;

namespace RobotCastle.Battling.MerchantOffer
{
    [CreateAssetMenu(menuName = "SO/Merchant Offer Config Container", fileName = "Merchant Offer Config", order = 460)]
    public class MerchantOfferConfigContainer : ScriptableObject
    {
        public MerchantOfferConfig config;
    }
}