using DG.Tweening;
using UnityEngine;

namespace RobotCastle.UI
{
    public class SideMoveAnimator : MonoBehaviour
    { 
        [SerializeField] private RectTransform _rect;
        [SerializeField] private float _inTime;
        [SerializeField] private float _outTime;
        [SerializeField] private Vector2 _inPos;
        [SerializeField] private Vector2 _outPos;

        public void On() => gameObject.SetActive(true);
        public void Off() => gameObject.SetActive(false);

        public void SetInPos()
        {
            _rect.DOKill();
            _rect.anchoredPosition = _inPos;
        }
        
        public void SetOutPos()
        {
            _rect.DOKill();
            _rect.anchoredPosition = _outPos;
        }

        public void AnimateIn()
        {
            On();
            _rect.DOKill();
            _rect.DOAnchorPos(_inPos, _inTime);
        }

        public void AnimateOut()
        {
            _rect.DOKill();
            _rect.DOAnchorPos(_outPos, _outTime).OnComplete(Off);
        }

    }
}