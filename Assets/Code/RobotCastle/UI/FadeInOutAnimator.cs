using DG.Tweening;
using UnityEngine;

namespace RobotCastle.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeInOutAnimator : MonoBehaviour
    {
        [SerializeField] private float _inTime;
        [SerializeField] private float _outTime;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        
        public void FadeIn()
        {
            On();
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, _inTime);
        }

        public void FadeOut()
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            _canvasGroup.DOFade(0f, _inTime).OnComplete(Off);
        }

        public void Off() => gameObject.SetActive(false);

        public void On() => gameObject.SetActive(true);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        #endif
    }
}