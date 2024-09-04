using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class SlowMotionManager : MonoBehaviour, ISlowMotionManager
    {
        private float _timeScale = 1f;
        private float _physycsTimeDelta = 1 / 50f;
        private Coroutine _timeChanging;


        public static SlowMotionManager Inst { get; private set; }

        private void Awake()
        {
            if (Inst == null)
                Inst = this;
            else
                Destroy(this);
        }

        public void Begin(SlowMotionEffect effect)
        {
            StopTimeChange();
            if (effect.EnterTime <= 0)
            {
                _timeScale = effect.TimeScale;
                if(effect.ScalePhysics)
                    SetTimeAndPhysicsScale();
                else
                    SetTimeScale();
                return;
            }
            _timeChanging = StartCoroutine(TimeChangingTo(effect.TimeScale, effect.EnterTime, effect.ScalePhysics));
            if(effect.Duration > 0)
                StartCoroutine(DelayedReturnToNormal(effect));
        }

        public void Exit(SlowMotionEffect effect)
        {
            StopTimeChange();
            if (_timeScale == 1f)
                return;
            if (effect.ExitTime <= 0)
            {
                _timeScale = 1f;
                if(effect.ScalePhysics)
                    SetTimeAndPhysicsScale();
                else
                    SetTimeScale();
                return;
            }
            _timeChanging = StartCoroutine(TimeChangingTo(1f, effect.ExitTime, effect.ScalePhysics));
        }

        public void SetNormalTime()
        {
            StopTimeChange();
            _timeScale = 1f;
            SetTimeAndPhysicsScale();
        }

        private void SetTimeScale()
        {
            Time.timeScale = _timeScale;
        }

        private void SetTimeAndPhysicsScale()
        {
            Time.timeScale = _timeScale;
            Time.fixedDeltaTime = _physycsTimeDelta * _timeScale;
        }

        private void StopTimeChange()
        {
            StopAllCoroutines();
            // if( _timeChanging != null)
                // StopCoroutine(_timeChanging);
        }
        
        private IEnumerator DelayedReturnToNormal(SlowMotionEffect effect)
        {
            yield return new WaitForSeconds(effect.Duration);
            Exit(effect);
        }
        
        private IEnumerator TimeChangingTo(float endScale, float time, bool physics)
        {
            var elapsed = 0f;
            var startScale = _timeScale;

            while (elapsed < time)
            {
                _timeScale = Mathf.Lerp(startScale, endScale, elapsed / time);
                if (physics)
                    SetTimeAndPhysicsScale();
                else
                    SetTimeScale();
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            _timeScale = endScale;
            if (physics)
                SetTimeAndPhysicsScale();
            else
                SetTimeScale();
        }
    }
}