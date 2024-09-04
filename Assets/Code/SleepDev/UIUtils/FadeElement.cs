using UnityEngine;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
namespace SleepDev
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeElement : MonoBehaviour
    {
        [SerializeField] private float _delay;
        [SerializeField] private float _durationIn;
        [SerializeField] private float _durationOut;
#if HAS_DOTWEEN
        [SerializeField] private Ease _ease;
#endif
        public float delay
        {
            get => _delay;
            set => _delay = value;
        }

        public float durationIn
        {
            get => _durationIn;
            set => _durationIn = value;
        }
        
        public float durationOut
        {
            get => _durationOut;
            set => _durationOut = value;
        }
#if HAS_DOTWEEN
        public Ease ease
        {
            get => _ease;
            set => _ease = value;
        }
#endif
        [SerializeField] private CanvasGroup _canvasGroup;

        
#if HAS_DOTWEEN
        private Tween _tween;
#endif
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
        }
#endif

        public void FadeIn()
        {
            _canvasGroup.alpha = 0f;
#if HAS_DOTWEEN
            _tween?.Kill();
            _tween = _canvasGroup.DOFade(1f, _durationIn).SetEase(_ease).SetDelay(_delay);
#endif
}

        public void FadeOut()
        {
            _canvasGroup.alpha = 1f;
#if HAS_DOTWEEN
            _tween?.Kill();
            _tween = _canvasGroup.DOFade(0f, _durationOut).SetEase(_ease).SetDelay(_delay);
        #endif
}

        public void SetIn()
        {
            _canvasGroup.alpha = 1f;
        }
        
        public void SetOut()
        {
            _canvasGroup.alpha = 0f;
        }

    }
}