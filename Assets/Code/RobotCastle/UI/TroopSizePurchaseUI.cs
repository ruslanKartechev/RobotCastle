using System;
using RobotCastle.Battling;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class TroopSizePurchaseUI : MonoBehaviour
    {
        [SerializeField] private MyButton _purchaseBtn;
        [SerializeField] private TextMeshProUGUI _priceText;
        private ITroopSizeManager _troopSizeManager;

        public void SetInteractable(bool allow) => _purchaseBtn.SetInteractable(allow);
        
        public void Init(ITroopSizeManager troopSizeManager)
        {
            _troopSizeManager = troopSizeManager;
            _priceText.text = $"{_troopSizeManager.GetCost()}";
            _purchaseBtn.AddMainCallback(OnBtn);
        }

        private void OnBtn()
        {
            _troopSizeManager.TryPurchase();
            _priceText.text = $"{_troopSizeManager.GetCost()}";
        }
        

        private void OnEnable()
        {
            if (_troopSizeManager != null)
            {
                _priceText.text = $"{_troopSizeManager.GetCost()}";
                
            }
        }
    }
}