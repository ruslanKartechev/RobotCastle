using System;
using SleepDev.Data;
using TMPro;
using UnityEngine;

namespace SleepDev.UIElements
{
    public class MoneyUI : MonoBehaviour
    {
        public bool subOnEnable;
        public ReactiveInt moneySource;
        
        protected bool _didSub;
        
        public virtual void Init(ReactiveInt source)
        {
            this.moneySource = source;
        }

        public virtual void DoReact(bool react)
        {
            if (react && !_didSub)
            {
                _didSub = true;
                this.moneySource.OnSet += OnSet;
                this.moneySource.OnUpdated += OnUpdated;
                this.moneySource.OnUpdatedWithContext += OnUpdatedContext;
            }
            else if(!react && _didSub)
            {
                _didSub = false;
                this.moneySource.OnSet -= OnSet;
                this.moneySource.OnUpdated -= OnUpdated;
                this.moneySource.OnUpdatedWithContext -= OnUpdatedContext;
            }
        }

        protected virtual void OnSet(int newVal, int prevVal)
        {
            _text.text = newVal.ToString();
        }
        
        protected virtual void OnUpdated(int newVal, int prevVal)
        {
            _text.text = newVal.ToString();
        }
        
        protected virtual void OnUpdatedContext(int newVal, int prev, int context)
        {
            _text.text = newVal.ToString();
        }
        
        [SerializeField] protected TextMeshProUGUI _text;

        protected virtual void OnDisable()
        {
            if (moneySource != null && _didSub)
            {
                _didSub = false;
                this.moneySource.OnSet -= OnSet;
                this.moneySource.OnUpdated -= OnUpdated;
                this.moneySource.OnUpdatedWithContext -= OnUpdatedContext;
            }
        }

        protected virtual void OnEnable()
        {
            if (subOnEnable && moneySource != null && !_didSub)
            {
                DoReact(true);
            }
        }
    }
}