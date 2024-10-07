using RobotCastle.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Summoning
{
    public class PurchaseButton : MyButton
    {
        [SerializeField] private Image _currencyIcon;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private Sprite _moneySprite;
        [SerializeField] private Sprite _itemSprite;
        
        public void On() => gameObject.SetActive(true);

        public void Off() => gameObject.SetActive(false);
        
        public void SetPriceEnough(int cost)
        {
            _currencyIcon.sprite = _moneySprite;
            _costText.text = $"<color=#FFFFFF>{cost}</color>";
        }

        public void SetPriceNotEnough(int cost)
        {
            _currencyIcon.sprite = _moneySprite;
            _costText.text = $"<color=#FF1111>{cost}</color>";
        }

        public void SetAsUseItem(int count)
        {
            _currencyIcon.sprite = _itemSprite;
            _costText.text = $"x{count}";
        }
    }
}