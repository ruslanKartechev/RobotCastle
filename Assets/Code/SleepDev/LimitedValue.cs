using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public class LimitedValue
    {
        [SerializeField] private float _limit_1;
        [SerializeField] private float _limit_2;
        [SerializeField] private float _value_1;
        [SerializeField] private float _value_2;

        public float MinValue => _value_1;
        public float MaxValue => _value_2;
        
        
        public void SetMaxValue(float val)
        {
            _value_2 = val;
        }
        
        public void SetMinValue(float val)
        {
            _value_1 = val;
        }


        public float GetValue(float input)
        {
            var t = Mathf.InverseLerp(_limit_1, _limit_2, input);
            return Mathf.Lerp(_value_1, _value_2, t);
        }
    }
}