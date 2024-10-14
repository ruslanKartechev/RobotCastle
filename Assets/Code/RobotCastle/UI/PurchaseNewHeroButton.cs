using RobotCastle.Battling;
using RobotCastle.Core;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class PurchaseNewHeroButton : MyButton
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        public void Init()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            gm.OnMoneySet += OnMoneyUpdated;
            OnMoneyUpdated(gm.levelMoney, 0);
        }
        
        private void OnMoneyUpdated(int newval, int prevval)
        {
            var cost = ServiceLocator.Get<IPlayerMergeItemsFactory>().NextCost;
            if (newval >= cost)
                _text.text = $"<color=#FFFFFF>{newval}</color>";
            else
                _text.text = $"<color=#FF1111>{newval}</color>";
        }
    }
}