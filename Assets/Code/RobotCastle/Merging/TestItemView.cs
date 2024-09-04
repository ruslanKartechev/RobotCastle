using SleepDev;
using UnityEngine;

namespace RobotCastle.Merging
{
    public class TestItemView : MonoBehaviour, IItemView
    {
        [SerializeField] private StarsLevelView _starsLevelView;
        private ItemData _data;
        
        public ItemData Data
        {
            get => _data;
            set
            {
                if (_data == value)
                    return;
                _data = value;
                if(_starsLevelView!=null)
                    _starsLevelView.SetLevel(_data.core.level);
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

        public void UpdateViewToData(ItemData data)
        {
            _data = data;
            if (_starsLevelView != null)
            {
                _starsLevelView.SetLevel(data.core.level);
                _starsLevelView.AnimateUpdated();                
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
    
    
    
}