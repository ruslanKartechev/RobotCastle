using SleepDev.Data;

namespace SleepDev.UIElements
{
    public class MoneyToBuyUI : MoneyUI
    {
        public ReactiveInt costSource;

        public virtual void InitWithCost(ReactiveInt moneySource, ReactiveInt costSource)
        {
            base.Init(moneySource);
            this.costSource = costSource;
            SetMoneyColorToCost(base.moneySource.Val, costSource.Val);
        }

        public override void DoReact(bool react)
        {
            if(react && !_didSub)
            {
                costSource.OnSet += OnCostSet;
                costSource.OnUpdated += OnCostSet;
        
            }
            else if(!react && !_didSub)
            {
                costSource.OnSet -= OnCostSet;
                costSource.OnUpdated -= OnCostSet;
            }
            base.DoReact(react);
        }

        protected virtual void OnCostSet(int newval, int prevval)
        {
            SetMoneyColorToCost(moneySource.Val, newval);
        }

        protected override void OnSet(int newVal, int prevVal)
        {
            SetMoneyColorToCost(newVal, costSource.Val);
        }
        
        protected override void OnUpdated(int newVal, int prevVal)
        {
            SetMoneyColorToCost(newVal, costSource.Val);
        }
        
        protected override void OnUpdatedContext(int newVal, int prev, int context)
        {
            SetMoneyColorToCost(newVal, costSource.Val);
        }
        
        protected virtual void SetMoneyColorToCost(int money, int cost)
        {
            if (money >= cost)
                _text.text = $"<color=#FFFFFF>{money}</color>";
            else
                _text.text = $"<color=#FF1111>{money}</color>";
        }
        
        
        protected override void OnDisable()
        {
            base.OnDisable();
            if (costSource != null && _didSub)
            {
                _didSub = false;
                this.costSource.OnSet -= OnCostSet;
                this.costSource.OnUpdated -= OnCostSet;
            }
        }
    }
}