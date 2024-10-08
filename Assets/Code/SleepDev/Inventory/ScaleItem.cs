using DG.Tweening;
using UnityEngine;

namespace SleepDev.Inventory
{
    public class ScaleItem : BasicItem
    {
        public const float ScaleTime = .25f;
        
        [SerializeField] protected float _scaleUp = 1.1f;
        [SerializeField] protected RectTransform _target;
        [SerializeField] protected GameObject _darkening;
        protected Tween _scaling;
        
        public override void Pick()
        {
            base.Pick();
            _target.gameObject.SetActive(true);
            _scaling?.Kill();
            _target.localScale = Vector3.one;
            _scaling = _target.DOScale(_scaleUp, ScaleTime);
            _darkening.SetActive(false);
        }

        public override void Unpick()
        {
            base.Unpick();
            _scaling?.Kill();
            _scaling = _target.DOScale(Vector3.one, ScaleTime);
            _darkening.SetActive(true);
        }
    }
}