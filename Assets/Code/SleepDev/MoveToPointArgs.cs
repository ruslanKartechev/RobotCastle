using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public class MoveToPointArgs
    {
        public float moveTime;
        public float rotTime;
        public AnimationCurve moveCurve;
        public AnimationCurve rotationCurve;
    }
}