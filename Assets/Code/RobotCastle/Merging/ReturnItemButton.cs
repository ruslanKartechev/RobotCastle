using DG.Tweening;
using RobotCastle.UI;
using TMPro;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class ReturnItemButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private FadeInOutAnimator _animator;
        [SerializeField] private Transform _scalabe;
        [SerializeField] private Vector2 _scaleMinMax;
        [SerializeField] private Vector2 _alphaMinMax;
        [SerializeField] private CanvasGroup _btnCanvasGroup;
        private Vector3[] corners = new Vector3[4];
        private bool _active;

        public bool IsAbove()
        {
            _rect.GetWorldCorners(corners);
            // CLog.Log($"Low left: {corners[0]},{corners[1]}, {corners[2]}, {corners[3]}");
            var pos = Input.mousePosition;
            if (pos.x >= corners[0].x && pos.x <= corners[2].x 
                                      && pos.y >= corners[0].y && pos.y <= corners[2].y)
            {
                return true;
            }
            return false;
        }

        public void ScaleOn(bool on)
        {
            if (_active == on)
                return;
            _active = on;
            var scale = on ? _scaleMinMax.y : _scaleMinMax.x;
            _scalabe.DOKill();
            _scalabe.DOScale(scale, .25f);
            _btnCanvasGroup.alpha = on ? _alphaMinMax.y : _alphaMinMax.x;
        }
        
        public void SetMoney(int amount) => _costText.text = $"+{amount}";

        public void Show()
        {
            if(!gameObject.activeInHierarchy)
                SetNotActive();
            _animator.FadeIn();
        }

        public void Hide()
        {
            if (gameObject.activeInHierarchy)
            {
                _animator.FadeOut();
                SetNotActive();
            }
        }

        private void SetNotActive()
        {
            _active = false;
            _scalabe.DOKill();
            _scalabe.localScale = _scaleMinMax.x * Vector3.one;
            _btnCanvasGroup.alpha = _alphaMinMax.x;
        }
    }
}