using UnityEngine;
#if HAS_DOTWEEN
using DG.Tweening;
#endif
namespace SleepDev
{
    public class ScalePulser : MonoBehaviour
    {
        [SerializeField] private float _magnitude;
        [SerializeField] private float _time;
        [SerializeField] private bool _autoStart;
        [SerializeField] private Transform _target;
        #if HAS_DOTWEEN
        private Sequence _sequence;
        #endif
        public Transform Target => _target;

        private void OnEnable()
        {
            if (_autoStart == false)
                return;
            Begin();
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Begin()
        {
            var startScale = _target.localScale.x;
            Stop();
            _target.localScale = Vector3.one * (startScale - _magnitude);
#if HAS_DOTWEEN
            _sequence = DOTween.Sequence();
            _sequence.Append(_target.DOScale(Vector3.one * (startScale + _magnitude), _time));
            _sequence.Append(_target.DOScale(Vector3.one * (startScale - _magnitude), _time));
            _sequence.SetLoops(-1);
#endif
        }

        public void Stop()
        {
#if HAS_DOTWEEN
            _sequence?.Kill();
#endif
}

        public void Reset()
        {
            Stop();
            _target.localScale = Vector3.one;
        }
    }
}