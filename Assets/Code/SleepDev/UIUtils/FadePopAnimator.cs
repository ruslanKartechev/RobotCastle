using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace SleepDev
{
    public class FadePopAnimator : MonoBehaviour
    {
        [SerializeField] private Ease _scaleEase;
        [SerializeField] private float _scaleTime;
        [SerializeField] private RectTransform _rect;
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
            _rect.DOKill();
            _rect.localScale = new Vector3(1f, 0f, 1f);
            _rect.DOScaleY(1f, _scaleTime).SetEase(_scaleEase);
            var elapsed = 0f;
            var time = _fadeTime;
            while (elapsed < time)
            {
                var t = elapsed / time;
                _canvasGroup.alpha = _curve.Evaluate(t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _canvasGroup.alpha = _curve.Evaluate(1f);
            if(_controlBlockState)
                _block.SetActive(false);
        }

    }
}