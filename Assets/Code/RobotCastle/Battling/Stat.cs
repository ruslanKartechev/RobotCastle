using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Battling
{
    public delegate void StatEventHandler(Stat stat);
    
    [System.Serializable]
    public class Stat : IFloatGetter
    {
        public event StatEventHandler OnValueChange;
        public event StatEventHandler OnDecoratorAdded;
        public event StatEventHandler OnDecoratorRemoved;
        

        private readonly List<IStatDecorator> _decorators = new List<IStatDecorator>(10);
        private readonly List<IStatDecorator> _permanentDecorators = new List<IStatDecorator>(10);

        private float _base;
        private float _val;

        public float BaseVal
        {
            get => _base;
            set
            {
                _base = value;
                OnValueChange?.Invoke(this);
            }
        }

        public float Val
        {
            get => _val;
            set
            {
                _val = value;
                OnValueChange?.Invoke(this);
            }
        }

        public void AddBaseAndCurrent(float added)
        {
            _val += added;
            _base += added;
            OnValueChange?.Invoke(this);
        }

        public void SetBase() => Val = BaseVal;

        public void SetBaseAndCurrent(float val)
        {
            BaseVal = Val = val;
            OnValueChange?.Invoke(this);
        }

        /// <summary>
        /// Get Full Value with all decorators applied
        /// </summary>
        /// <returns></returns>
        public float Get()
        {
            var v = Val;
            foreach (var dec in _permanentDecorators)
                v = dec.Decorate(v);
            foreach (var dec in _decorators)
                v = dec.Decorate(v);
            return v;
        }

        public void ClearDecorators()
        {
            _decorators.Clear();
        }

        public void AddDecorator(IStatDecorator decorator)
        {
            if (_decorators.Contains(decorator))
                return;
            _decorators.Add(decorator);
            OnDecoratorAdded?.Invoke(this);
        }

        public void RemoveDecorator(IStatDecorator decorator)
        {
            _decorators.Remove(decorator);
            OnDecoratorRemoved?.Invoke(this);
        }

        public void AddPermanentDecorator(IStatDecorator decorator)
        {
            _permanentDecorators.Add(decorator);
            OnDecoratorAdded?.Invoke(this);
        }

        public void RemovePermanentDecorator(IStatDecorator decorator)
        {
            _permanentDecorators.Remove(decorator);
            OnDecoratorRemoved?.Invoke(this);
        }
        
        
        public string GetAllDecoratorsAsStr()
        {
            var msg = $"Count: {_decorators.Count}; ";
            foreach (var dec in _decorators)
                msg += $"{dec.name} ";
            return msg;
        }
    }
    
}