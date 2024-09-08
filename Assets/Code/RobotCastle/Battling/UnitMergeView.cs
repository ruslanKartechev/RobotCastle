using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitMergeView : MonoBehaviour, IItemView
    {
        [SerializeField] private UnitView _unitView;
        private ItemData _data;
        
        public ItemData itemData
        {
            get => _data;
            set
            {
                if (_data == value)
                    return;
                _data = value;
                // _starsLevelView.levelView.SetLevel(_data.core.level);
            } 
        }

        public Transform Transform => transform;

        public void OnPicked()
        { }

        public void OnPut()
        { }

        public void OnDroppedBack()
        { }

        public void OnMerged()
        { }

        public void UpdateViewToData(ItemData data = null)
        {
            if (data != null)
                _data = data;
            if (_unitView != null)
            {
                _unitView.UnitUI.Level.SetLevel(_data.core.level);
                _unitView.UnitUI.Level.AnimateUpdated();                
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}