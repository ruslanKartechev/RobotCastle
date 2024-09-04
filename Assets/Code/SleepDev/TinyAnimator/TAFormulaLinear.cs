namespace SleepDev.TinyAnimator
{
    public class TAFormulaLinear : ITAFormula
    {
        private float _factor;

        public float Factor
        {
            get => _factor;
            set => _factor = value;
        }

        public float GetValue(float interpolationValue) => _factor * interpolationValue;

        public TAFormulaLinear(float factor)
        {
            _factor = factor;
        }
        
    }
}