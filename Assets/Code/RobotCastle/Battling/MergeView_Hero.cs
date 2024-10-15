using System.Threading.Tasks;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class MergeView_Hero : MonoBehaviour, IItemView
    {
        [SerializeField] private HeroComponents _view;
        private ItemData _data;
        private Tween _tweenScale;
        private Tween _tweenMove;
        
        
        public ItemData itemData
        {
            get => _data;
            set => _data = value;
        }

        public Transform Transform => transform;

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
            transform.DORotateQuaternion(rotation, time);
        }

        public void MoveToPoint(Transform endPoint, float time)
        {
            _tweenMove?.Kill();
            _tweenMove = transform.DOMove(endPoint.position, time);
            transform.DORotateQuaternion(endPoint.rotation, time);
        }

        public async void InitView(ItemData data)
        {
            _data = data;
            var scale = transform.localScale;
            transform.localScale = scale * .8f;
            transform.DOScale(scale, .3f);
            while (_view.heroUI == null)
                await Task.Yield();
            _view.heroUI.Level.SetLevel(_data.core.level);
            _view.heroUI.Level.AnimateUpdated();  
        }

        public void UpdateViewToData(ItemData data = null)
        {
            if (data != null)
                _data = data;
            _view.heroUI.Level.SetLevel(_data.core.level);
            _view.heroUI.Level.AnimateUpdated();     
            _view.stats.SetMergeLevel(mergeLevel: _data.core.level);
            if(gameObject.TryGetComponent(out IHeroController hero))
                hero.View.heroUI.UpdateStatsView(hero.View);
        }

        public void Hide()
        {
            _view.heroUI.GetComponent<HeroUIWrapper>().ReturnBack();
            gameObject.SetActive(false);
        }
    }
}