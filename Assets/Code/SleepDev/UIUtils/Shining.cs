using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SleepDev
{
    public class Shining : MonoBehaviour
    {
        [SerializeField] private bool _autoStart;
        [Space(10)]
        [SerializeField] private Image _icon;
        [SerializeField] private Transform _rotatable;
        [SerializeField] private float _rotationSpeed;
        [Space(10)]
        [SerializeField] private float _alphaHalfPeriod;
        [SerializeField] private float _alphaMin;
        [SerializeField] private float _alphaMax;
        private Coroutine _rotating;
        private Coroutine _alphaChange;
        
        
        private void Start()
        {
            if(_autoStart)
                Begin();
        }

        public void Begin()
        {
            Stop();
            _rotating = StartCoroutine(Rotating());
            _alphaChange = StartCoroutine(AlphaShining());
        }

        public void Stop()
        {
            if(_alphaChange != null)
                StopCoroutine(_alphaChange);
            if(_rotating != null)
                StopCoroutine(_rotating);
        }

        private IEnumerator Rotating()
        {
            while (true)
            {
                var angles = _rotatable.eulerAngles;
                angles.z += Time.deltaTime * _rotationSpeed;
                _rotatable.eulerAngles = angles;
                yield return null;
            }
        }
        
        
        private IEnumerator AlphaShining()
        {
            var elapsed = 0f;
            var time = _alphaHalfPeriod;
            while (true)
            {
                elapsed = 0;
                while (elapsed <= time)
                {
                    var color = _icon.color;
                    color.a = Mathf.Lerp(_alphaMin, _alphaMax, elapsed / time);
                    _icon.color = color;
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                elapsed = 0;
                while (elapsed <= time)
                {
                    var color = _icon.color;
                    color.a = Mathf.Lerp(_alphaMax, _alphaMin, elapsed / time);
                    _icon.color = color;
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                yield return null;
            }
        }
        
        
        
    }
}