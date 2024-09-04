using UnityEngine;
#if HAS_DOTWEEN
using DG.Tweening;
#endif

namespace SleepDev
{
    public class SimplePopElement : PopElement
    {
        [SerializeField] private float _delay;
        [SerializeField] private float _duration;
        [SerializeField] private float _scale = 1f;
#if HAS_DOTWEEN
        [SerializeField] private Ease _ease;
#endif
        public override float Delay
        {
            get => _delay;
            set => _delay = value;
        }

        public override float Duration
        {
            get => _duration;
            set => _duration = value;
        }
#if HAS_DOTWEEN
        public Ease Ease
        {
            get => _ease;
            set => _ease = value;
        }
#endif
        public float Scale
        {
            get => _scale;
            set => _scale = value;
        }
        
        public override void ScaleUp()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
#if HAS_DOTWEEN
            transform.DOScale(Vector3.one * _scale, _duration).SetEase(_ease).SetDelay(_duration);
        #endif
}
        
        public override void ScaleDown()
        {
            transform.localScale = Vector3.one * _scale;
#if HAS_DOTWEEN
            transform.DOScale(Vector3.zero, _duration).SetEase(_ease).SetDelay(_duration);
        #endif
        }

    }
}