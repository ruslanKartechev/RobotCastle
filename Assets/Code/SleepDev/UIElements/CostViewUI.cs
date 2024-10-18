namespace SleepDev.UIElements
{
    public class CostViewUI : MoneyToBuyUI
    {

        protected override void SetMoneyColorToCost(int money, int cost)
        {
            if (money >= cost)
                _text.text = $"<color=#FFFFFF>{cost}</color>";
            else
                _text.text = $"<color=#FF1111>{cost}</color>";
        }

    }
}