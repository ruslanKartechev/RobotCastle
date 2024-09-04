using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class CameraShaker : MonoBehaviour, ICameraShaker
    {
        [SerializeField] private CameraShakeArgs _defaultArgs;
        [SerializeField] private Transform _movable;
        private Coroutine _working;
        public void Play(CameraShakeArgs args)
        {
            Stop();
            _working = StartCoroutine(Working(args));
        }

        public void PlayDefault()
        {
            Play(_defaultArgs);   
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Working(CameraShakeArgs args)
        {
            var elapsed = 0f;
            var timeStep = 1f / args.freqDefault;
            var force = args.forceDefault;
            var forceMax = force;
            var forceMin = force * .25f;
            while (elapsed <= args.durationDefault)
            {
                var eulers = (Vector3)UnityEngine.Random.insideUnitCircle * force;
                _movable.localRotation = Quaternion.Euler(eulers);
                yield return new WaitForSeconds(timeStep);
                elapsed += timeStep;
                force = Mathf.Lerp(forceMin, forceMax, elapsed / args.durationDefault);
            }
            _movable.localRotation = Quaternion.identity;
        }
    }
}