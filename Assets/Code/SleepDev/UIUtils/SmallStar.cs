using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class SmallStar : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private float _endSize = -1;
        [SerializeField] private float _startSize = 0;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _scaleTime;
        private Coroutine _working;
        
        
        public void Begin()
        {
            StartCoroutine(Rotating());
            StartCoroutine(Scaling());
        }
        
        public void Stop()
        {
            StopAllCoroutines();
        }

        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rect == null)
                _rect = GetComponent<RectTransform>();
            if (_rect != null && _endSize <= 0)
                _endSize = _rect.localScale.x;
        }
#endif
        private void OnEnable()
        {
            Begin();
        }

        private IEnumerator Rotating()
        {
            while (true)
            {
                var angles = _rect.localEulerAngles;
                angles.z += Time.deltaTime * _rotationSpeed;
                _rect.localEulerAngles = angles;
                yield return null;
            }      
        }

        private IEnumerator Scaling()
        {
            var elapsed = 0f;
            var time = _scaleTime * 2;
            while (true)
            {
                elapsed = 0f;
                while (elapsed <= time)
                {
                    _rect.localScale = Vector3.one * Mathf.Lerp(_endSize, _startSize, elapsed / time);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                elapsed = 0f;
                while (elapsed <= time)
                {
                    _rect.localScale = Vector3.one * Mathf.Lerp(_startSize, _endSize, elapsed / time);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                _rect.localScale = Vector3.one * _endSize;
                yield return null;
            }      
        }        
        
    }
}