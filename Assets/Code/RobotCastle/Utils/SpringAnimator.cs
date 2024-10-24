using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobotCastle.Utils
{
    public class SpringAnimator : MonoBehaviour
    {
        public float mass
        {
            get => _mass;
            set => _mass = value;
        }

        public float spring
        {
            get => _spring;
            set => _spring = value;
        }

        public float damping
        {
            get => _damping;
            set => _damping = value;
        }

        public virtual void OnUpdated(float val)
        {
            
        }
        
        [ContextMenu("Begin Animating From Start")]
        public void BeginAnimatingFromStart()
        {
            foreach (var ll in _listeners)
                ll.OnStarted();
            _time = 0f;
            StopRoutine();
            _working = StartCoroutine(Calculating());
        }

        public void Stop()
        {
            StopRoutine();
            foreach (var ll in _listeners)
                ll.OnStopped();
        }


        [SerializeField] protected bool _autoStartOnEnable;
        [SerializeField] protected float _mass = 1;
        [SerializeField] protected float _spring = 100;
        [SerializeField] protected float _damping = 1f;
        [SerializeField] protected List<SpringAnimatorListener> _listeners;
        protected Coroutine _working;
        protected float _currentVal;
        protected float _time = 0f;
        
        protected void StopRoutine()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        protected IEnumerator Calculating()
        {
            const float threshold = .01f;
            var doLoop = true;
            while (doLoop)
            {
                var br = _damping / (2 * _mass);
                var w2 = _spring / _mass - br * br;
                var amp = Mathf.Exp(-1 * _damping * _time / (2 * _mass));
                var val = amp * Mathf.Cos(w2 * _time);
                _currentVal = val;
                foreach (var ll in _listeners)
                    ll.OnUpdated(val);
                doLoop = Mathf.Abs(amp) > threshold;
                _time += Time.deltaTime;
                yield return null;
            }
        }

        protected void OnEnable()
        {
            if(_autoStartOnEnable)
                BeginAnimatingFromStart();
        }
    }
}