using DG.Tweening;
using UnityEngine;

namespace SleepDev
{
    public class TopBottomScrollElement : ScrollElement
    {
        private const float moveTime = .3f;
        private const float moveOffset = 15f;
        
        [SerializeField] private RectTransform _bottom;
        [SerializeField] private RectTransform _pointBottom;
        [SerializeField] private GameObject _pointHighlight;
        
        public override void Close(bool leftToRight)
        {
            Off();
            
        }

        public override void Show(bool leftToRight)
        {
            On();
            // var anch = _pointTop.anchoredPosition + Vector2.up * moveOffset;
            // _top.anchoredPosition = anch;
            // _top.DOMove(_pointTop.position, moveTime);
            var anch = _pointBottom.anchoredPosition - Vector2.up * moveOffset;
            _bottom.anchoredPosition = anch;
            _bottom.DOMove(_pointBottom.position, moveTime);
        }

        public override void On()
        {
            // _top.gameObject.SetActive(true);
            _bottom.gameObject.SetActive(true);
            _pointHighlight.SetActive(true);
        }

        public override void Off()
        {
            // _top.gameObject.SetActive(false);
            _bottom.gameObject.SetActive(false);
            _pointHighlight.SetActive(false);
        }
    }
}