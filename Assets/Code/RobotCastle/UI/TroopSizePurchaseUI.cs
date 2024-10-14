using System;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class TroopSizePurchaseUI : MonoBehaviour
    {
        [SerializeField] private MyButton _purchaseBtn;
        [SerializeField] private TextMeshProUGUI _priceText;
        private ITroopSizeManager _troopSizeManager;
        private bool _didSub;

        public void SetInteractable(bool allow) => _purchaseBtn.SetInteractable(allow);
        
        public void Init(ITroopSizeManager troopSizeManager)
        {
            _troopSizeManager = troopSizeManager;
            var gm = ServiceLocator.Get<GameMoney>();
            if (!_didSub)
            {
                gm.OnMoneySet += OnMoneySet;
                _didSub = true;
            }
            OnMoneySet(gm.levelMoney, 0);
            _purchaseBtn.AddMainCallback(OnBtn);
        }

        private void OnMoneySet(int newval, int prevval)
        {
            var cost = _troopSizeManager.GetCost();
            if (newval >= cost)
                _priceText.text = $"<color=#FFFFFF>{cost}</color>";
            else
                _priceText.text = $"<color=#FF1111>{cost}</color>";
        }

        private void OnBtn()
        {
            var res = _troopSizeManager.TryPurchase();
            _priceText.text = $"{_troopSizeManager.GetCost()}";
            if (res == 1)
            {
                CLog.Log("Not enough money");
            }
        }

        private void OnEnable()
        {
            if (_troopSizeManager != null)
            {
                var gm = ServiceLocator.Get<GameMoney>();
                if (!_didSub)
                {
                    gm.OnMoneySet += OnMoneySet;
                    _didSub = true;
                }
                OnMoneySet(gm.levelMoney, 0);
            }
        }

        private void OnDisable()
        {
            if (_didSub)
            {
                var gm = ServiceLocator.Get<GameMoney>();
                gm.OnMoneySet -= OnMoneySet;
                _didSub = false;
            }
        }
    }
}