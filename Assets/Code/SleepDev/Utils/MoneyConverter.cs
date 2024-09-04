using UnityEngine;

namespace SleepDev
{
    public static class MoneyConverter
    {
        public static void InitCulture()
        {
            System.Globalization.CultureInfo customCulture = 
                (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        }
        
        public static  string Convert(float amount)
        {
            if (amount >= 1000000000f)
            {
                return $"{amount/1000000000f:N1}B";
            }
            else if (amount >= 1000000f)
            {
                if (amount % 1000000f == 0)
                    return $"{(int)(amount/1000000)}M";
                return $"{amount/1000000f:N1}M";
            }
            else if (amount >= 1000f)
            {
                if (amount % 1000f == 0)
                    return $"{(int)(amount/1000)}K";
                return $"{amount/1000f:N1}K";
            }
            return $"{Mathf.RoundToInt(amount)}";
        }
    }
}