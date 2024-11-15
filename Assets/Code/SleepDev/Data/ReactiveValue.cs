namespace SleepDev.Data
{
    

    public delegate void UpdateDelegate<T>(T newVal, T prevVal);
    
    public delegate void ContextUpdateDelegate<T>(T newVal, T prevVal, int context);

    public enum EMoneyChangeContext { SimpleSet, AfterPurchase, RewardAdded, CostUpdated, CostIncreased, CostDecreased, }
    
    public abstract class ReactiveValue<T> : IValueGetter<T>
    {
        public ReactiveValue() => this.val = default;

        public ReactiveValue(T val)
        {
            this.val = val;
        }
        
        public T Val => val;
        
        public T Get() => val;
    
        private T val;
        
        /// <summary>
        /// Raised every time a value is changed;
        /// </summary>
        public event UpdateDelegate<T> OnSet;
        /// <summary>
        /// Raise every time a value is added/subtracted
        /// </summary>
        public event UpdateDelegate<T> OnUpdated;

        public ContextUpdateDelegate<T> OnUpdatedWithContext;

        public void SetValue(T newVal)
        {
            var prevVal = val;
            this.val = newVal;
            OnSet?.Invoke(newVal, prevVal);
        }
        
        public void SetValueOnEvent(T newVal)
        {
            this.val = newVal;
        }
        
        public T AddValue(T addedVal)
        {
            var prevVal = this.val;
            val = Add(prevVal, addedVal);
            OnUpdated?.Invoke(val, addedVal);
            OnSet?.Invoke(val, prevVal);
            return val;
        }
        
        public void UpdateWithContext(T newValue, int context)
        {
            var prevVal = this.val;
            this.val = newValue;
            OnUpdatedWithContext?.Invoke(this.val, prevVal, context);
            OnSet?.Invoke(val, prevVal);
        }
        
        public void ResetListeners()
        {
            OnSet = null;
            OnUpdated = null;
        }

        protected abstract T Add(T a, T b);
    }
    
    public class ReactiveInt : ReactiveValue<int>
    {
        public ReactiveInt(){}

        public ReactiveInt(int val) : base(val) { }
        
        protected override int Add(int a, int b) => a + b;
    }
    
    
    public class ReactiveFloat : ReactiveValue<float>
    {
        public ReactiveFloat(){}

        public ReactiveFloat(float val) : base(val) { }
        protected override float Add(float a, float b) => a + b;
    }

    
    public class ReactiveString : ReactiveValue<string>
    {
        public ReactiveString(){}
        
        public ReactiveString(string val) : base(val) { }
        protected override string Add(string a, string b) => a + b;
    }

    
    
}