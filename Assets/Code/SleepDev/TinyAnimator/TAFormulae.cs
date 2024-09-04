namespace SleepDev.TinyAnimator
{
    
    public delegate float TAFormulaDelegate(float interpolationValue);

    public static class TAFormulae
    {
    
        public static float BellCurve00(float t)
        {
            return -4f * (t * t - t);
        }
        
        /// <summary>
        ///  = A * x^3 + B * x^2
        /// </summary>
        public static float Bounce(float t)
        {
            return (-5f * t * t * t + 6f * t * t);
        }

        public static float DefaultLine(float t) => t;

        
        /// <summary>
        /// Will drop below 0 at (.25, -0.125);
        /// </summary>
        public static float Quad01(float t)
        {
            return 2 * t * t - t;
        }
        
    }
}