using System;
using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class MonoExtended : MonoBehaviour
    {
        protected Coroutine _delayedAction;

        protected void DelayNoReturn(Action callback, float time)
        {
            StartCoroutine(DelayedAction(time, callback));
        }
        
        protected Coroutine Delay(Action callback, float time)
        {
            return StartCoroutine(DelayedAction(time, callback));
        }
        
        protected Coroutine DelayRealtime(Action callback, float time)
        {
            return StartCoroutine(DelayedActionRealtime(time, callback));
        }

        protected void StopDelayedAction()
        {
            if(_delayedAction != null)
                StopCoroutine(_delayedAction);
        }
        
        protected IEnumerator DelayedAction(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }
        
        protected IEnumerator DelayedActionRealtime(float time, Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action.Invoke();
        }
    }
}