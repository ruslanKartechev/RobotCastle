using DG.Tweening;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class MergeView_Hero : MonoBehaviour, IItemView
    {
        [SerializeField] private HeroView _view;
        private ItemData _data;
        
        public ItemData itemData
        {
            get => _data;
            set => _data = value;
        }

        public Transform Transform => transform;

        public void OnPicked() { }

        public void OnPut() { }

        public void OnDroppedBack() { }

        public void OnMerged()
        { }
        
        public void Rotate(Quaternion rotation, float time)
        {
            transform.DOKill();
            transform.DORotateQuaternion(rotation, time);
        }

        public void MoveToPoint(Transform endPoint, float time)
        {
            transform.DOKill();
            transform.DOMove(endPoint.position, time);
            transform.DORotateQuaternion(endPoint.rotation, time);
        }

        public void InitView(ItemData data)
        {
            _data = data;
            _view.heroUI.Level.SetLevel(_data.core.level);
            _view.heroUI.Level.AnimateUpdated();  
        }

        public void UpdateViewToData(ItemData data = null)
        {
            if (data != null)
                _data = data;
            _view.heroUI.Level.SetLevel(_data.core.level);
            _view.heroUI.Level.AnimateUpdated();     
            _view.Stats.SetMergeLevel(mergeLevel: _data.core.level);
            if(gameObject.TryGetComponent(out IHeroController hero))
                hero.View.heroUI.UpdateStatsView(hero.View);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}