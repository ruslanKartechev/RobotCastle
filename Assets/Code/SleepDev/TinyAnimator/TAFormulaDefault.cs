namespace SleepDev.TinyAnimator
{
    public class TAFormulaDefault : ITAFormula
    {
        public float GetValue(float interpolationValue) => interpolationValue;
    }
}