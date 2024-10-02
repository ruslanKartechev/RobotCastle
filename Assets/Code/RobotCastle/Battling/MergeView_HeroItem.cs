using DG.Tweening;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class MergeView_HeroItem : MonoBehaviour, IItemView
    {
        private ItemData _data;
        
        public ItemData itemData
        {
            get => _data;
            set => _data = value;
        }

        public Transform Transform => transform;

        private Tween _tweenScale;
        private Tween _tweenMove;
        
        public void OnPicked()
        {
            _tweenScale?.Kill();
            transform.localScale = Vector3.one;
            transform.DOScale(MergeConstants.PickScale, MergeConstants.PickScaleTime);
        }

        public void OnPut()
        {
            _tweenScale?.Kill();
            _tweenScale = transform.DOScale(Vector3.one, MergeConstants.PickScaleTime);
        }

        public void OnMerged()
        {
            _tweenScale?.Kill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * .2f, MergeConstants.PickScaleTime);
        }

        public void OnDroppedBack() 
        { 
            _tweenScale?.Kill();
            _tweenScale = transform.DOScale(Vector3.one, MergeConstants.PickScaleTime);
        }

        public void Rotate(Quaternion rotation, float time)
        {
        }

        public void MoveToPoint(Transform endPoint, float time)
        {
            _tweenMove?.Kill();
            _tweenMove = transform.DOMove(endPoint.position, time);
            transform.DORotateQuaternion(endPoint.rotation, time);
        }

        public void InitView(ItemData data)
        {
            _data = data;
            var scale = transform.localScale;
            transform.localScale = scale * .8f;
            transform.DOScale(scale, .3f);
        }

        public void UpdateViewToData(ItemData data)
        {
            _data = data;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}