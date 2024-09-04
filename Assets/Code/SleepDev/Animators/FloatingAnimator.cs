using System;
using UnityEngine;

namespace SleepDev
{
    [DefaultExecutionOrder(-1000)]
    public class FloatingAnimator : MonoBehaviour
    {
        public AnimationCurve curve;
        public float magnitude;
        public float halfPeriod;
        [NonSerialized] public float t;
        [NonSerialized] public float elapsed;

        private void Start()
        {
            t = UnityEngine.Random.Range(0f, 1f);
            elapsed = t * magnitude;
        }

        public void Update()
        {
            var pos = transform.localPosition;
            pos.y = magnitude * curve.Evaluate(t);
            transform.localPosition = pos;
            elapsed += Time.deltaTime;
            t = elapsed / halfPeriod;
            if (t >= 1)
                t = elapsed = 0f;
        }
    }
}