#if HAS_DOTWEEN
using DG.Tweening;
#endif
using UnityEngine;

namespace SleepDev
{
    public class JoystickUI : MonoBehaviour, IJoystickUI
    {
        [SerializeField] private float _maxRad;
        [SerializeField] private float _fadeTime;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _block;
        [SerializeField] private CanvasGroup _group;
        
        public void Show()
        {
            _group.gameObject.SetActive(true);
            _group.alpha = 0f;
#if HAS_DOTWEEN
            _group.DOKill();
            _group.DOFade(1f, _fadeTime);
    #endif
        }

        public void Hide()
        {
#if HAS_DOTWEEN
            _group.DOKill();
            _group.DOFade(0f, _fadeTime).OnComplete(() => { _group.gameObject.SetActive(false); });
#else
            _group.alpha = 0f;
#endif
        }

        public void HideNow()
        {
            _group.alpha = 0f;
        }

        public void SetPosition(Vector3 screenPosition)
        {
            _block.position = screenPosition;
        }

        public void SetJoystickLocal(Vector3 delta)
        {
            _movable.localPosition = delta;
        }

        public void SetJoystickLocal(Vector3 normalizedPos, float percent)
        {
            _movable.localPosition = normalizedPos * percent * _maxRad;
        }
    }
}