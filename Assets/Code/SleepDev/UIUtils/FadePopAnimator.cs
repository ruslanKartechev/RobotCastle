using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class FadePopAnimator : MonoBehaviour
    {
        [SerializeField] private float _fadeDelay;
        [SerializeField] private float _fadeTime;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private bool _controlBlockState = true;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _block;
        private Coroutine _working;
        
        public float fadeDelay
        {
            get => _fadeDelay;
            set => _fadeDelay = value;
        }

        public float fadeTime
        {
            get => _fadeTime;
            set => _fadeTime = value;
        }

        public bool controlBlockState
        {
            get => _controlBlockState;
            set => _controlBlockState = value;
        }
        
        public void Animate()
        {
            if (_controlBlockState)
                _block.SetActive(true);
            Stop();
            _working = StartCoroutine(Animating());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Animating()
        {
            // var alpha = 1f;
            // _canvasGroup.alpha = alpha;
            // yield return new WaitForSeconds(_fadeDelay);
            var elapsed = 0f;
            var time = _fadeTime;
            // var start = alpha;
            while (elapsed < time)
            {
                var t = elapsed / time;
                _canvasGroup.alpha = _curve.Evaluate(t);
                // _canvasGroup.alpha = Mathf.Lerp(start, 0f, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _canvasGroup.alpha = _curve.Evaluate(1f);
            if(_controlBlockState)
                _block.SetActive(false);
        }

    }
}