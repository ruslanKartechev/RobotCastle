using System;
using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class SimpleForwardMover : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private float _moveTime;
        private Coroutine _working;
        private Action _callback;

        public Transform Movable => _movable;
        
        public void Move(Action callback)
        {
            _callback = callback;
            _movable.gameObject.SetActive(true);
            Stop();
            _working = StartCoroutine(Moving(_endPoint, _moveTime));
        }
        
        public void Move(Transform point, float time, Action callback)
        {
            _callback = callback;
            _movable.gameObject.SetActive(true);
            Stop();
            _working = StartCoroutine(Moving(point, time));
        }
        
        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private IEnumerator Moving(Transform point, float time)
        {
            var elapsed = 0f;
            var p1 = _movable.position;
            var p2 = point.position;
            var t = 0f;
            while (t <= 1f)
            {
                _movable.position = Vector3.Lerp(p1, p2, t);
                elapsed += Time.deltaTime * _curve.Evaluate(t);
                t = elapsed / time;
                yield return null;
            }
            _movable.position = p2;
            _callback?.Invoke();
        }
    }
}