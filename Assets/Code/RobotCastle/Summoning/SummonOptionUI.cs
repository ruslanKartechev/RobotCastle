using System.Collections.Generic;
using RobotCastle.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Summoning
{
    public class SummonOptionUI : MonoBehaviour
    {
        public string id; 
        [Space(10)]
        public List<GameObject> doubleScrollObjects;
        public List<Image> purchaseImages;
        [Space(10)]
        public TextMeshProUGUI titleText;
        [Space(10)]
        public InventoryView invView1;
        public InventoryView invView2;
        [Space(10)]
        public PurchaseButton btnOptional;
        public PurchaseButton btn1;
        public PurchaseButton btn2;
        public PurchaseButton btn3;
        [Space(10)] 
        public float multiplier_1 = 1;
        public float multiplier_2 = 4;
        public float multiplier_3 = 20;
        
        
        [System.Serializable]
        public class InventoryView
        {
            public TextMeshProUGUI text;
            public Image costIcon;
        }
        
        public void SetPurchasesCount(int count)
        {
            for (var i = 0; i < purchaseImages.Count; i++)
            {
                purchaseImages[i].gameObject.SetActive(i < count);
            }
        }
    }
}