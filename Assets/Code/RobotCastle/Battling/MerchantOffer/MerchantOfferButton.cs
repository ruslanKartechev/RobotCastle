using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.Battling.MerchantOffer
{
    public class MerchantOfferButton : MonoBehaviour
    {
        public MyButton btnNormal;
        public MyButton btnAds;
        public TextMeshProUGUI costText;
        
        public MyButton activeBtn { get; private set; }

        public void SetAds()
        {
            btnNormal.gameObject.SetActive(false);
            btnAds.gameObject.SetActive(true);
            activeBtn = btnAds;
        }

        public void SetNonAds()
        {
            btnNormal.gameObject.SetActive(true);
            btnAds.gameObject.SetActive(false);
            activeBtn = btnNormal;
        }

        public void SetCost(int cost)
        {
            costText.text = cost.ToString();
        }

    }
}