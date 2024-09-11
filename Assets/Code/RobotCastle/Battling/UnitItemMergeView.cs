using DG.Tweening;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitItemMergeView : MonoBehaviour, IItemView
    {
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

        public void OnMerged() { }

        public void InitView(ItemData data)
        {
            _data = data;
            var scale = transform.localScale;
            transform.localScale = scale * .6f;
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