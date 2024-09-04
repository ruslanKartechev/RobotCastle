using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class VehiclePassBy : MonoBehaviour
    {
        [SerializeField] private bool _autoStart;
        [SerializeField] private float _moveTime;
        [SerializeField] private float _repeatDelay;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private Transform _startPoint;
        
        private Coroutine _working;
        
        private void Start()
        {
            if(_autoStart)
                Begin();
        }

        public void Begin()
        {
            Stop();
            _working = StartCoroutine(Working());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Working()
        {
            var vec = _movable.position - _startPoint.position;
            var vec2End = (_endPoint.position - _startPoint.position);
            var maxLength = vec2End.magnitude;
            vec2End /= maxLength;
            var dot = Vector3.Dot(vec, vec2End);
            var t = Mathf.InverseLerp(0f, maxLength, dot);
            var time = _moveTime;
            var elapsed = time * t;
            // CLog.LogBlue($"Dot {dot}, t {t}, max length {maxLength}");
            while (true)
            {
                _movable.gameObject.SetActive(true);
                while (t <= 1f)
                {
                    var l = Mathf.Lerp(0f, maxLength, t);
                    _movable.position = _startPoint.position + vec2End * l;
                    elapsed += Time.deltaTime;
                    t = elapsed / time;
                    
                    yield return null;
                }
                _movable.position = _endPoint.position;
                _movable.gameObject.SetActive(false);
                elapsed = t = 0f;
                yield return new WaitForSeconds(_repeatDelay);
            }
   
        }
    }
}