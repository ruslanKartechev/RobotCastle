using UnityEngine;

namespace SleepDev
{
    public static class Bezier
    {
        public const int DefaultSamples = 25;
        
        public static Vector3 GetPosition(Vector3 start, Vector3 inflection, Vector3 end, float t)
        {
            return ((1 - t) * (1 - t)) * start
                   + (2 * (1 - t) * t) * inflection
                   + (t * t) * end;
        }
        
        public static float GetValue(float start, float inflection, float end, float t)
        {
            return ((1 - t) * (1 - t)) * start
                   + (2 * (1 - t) * t) * inflection
                   + (t * t) * end;
        }

        public static float GetLength(Vector3 start, Vector3 inflection, Vector3 end, int samples)
        {
            var length2 = 0f;
            var prevPoint = start;
            for (var i = 1f; i <= samples; i++)
            {
                var p = GetPosition(start, inflection, end, i / samples);
                length2 += (p - prevPoint).sqrMagnitude;
                prevPoint = p;
            }
            return Mathf.Sqrt(length2);
        }
    }
}