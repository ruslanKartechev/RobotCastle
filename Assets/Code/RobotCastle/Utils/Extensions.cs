using System.Collections.Generic;

namespace RobotCastle.Utils
{
    public static class Extensions
    {
        public static int SecToMs(this float timeSec)
        {
            return (int)(timeSec * 1000);
        }

        public static List<T> ClearNulls<T>(this List<T> list) where T : class
        {
            var count = list.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                    list.RemoveAt(i);
            }
            return list;
        }
    }
}