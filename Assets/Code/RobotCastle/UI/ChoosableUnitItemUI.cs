using System;
using DG.Tweening;
using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ChoosableUnitItemUI : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _scalable;
        private bool _isChosen;

        public event Action OnStateChange;
        
        public bool IsChosen => _isChosen;
        public bool AllowChoose { get; set; } = true;
        
        public int Index { get; set; }
        
        public void SetIcon(Sprite sprite)
        {
            _icon.sprite = sprite;
        }

        public void Reset()
        {
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            _background.color = db.mergeItemIdleColor;
            _scalable.localScale = Vector3.one * db.mergeItemIdleScale;
            OnStateChange = null;
            _isChosen = false;
        }

        public void SetPicked()
        {
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            _background.color = db.mergeItemPickedColor;
            _scalable.DOKill();
            _scalable.DOScale(Vector3.one * db.mergeItemPickedScale, db.mergeItemScaleTime);
        }

        public void SetIdle()
        {
            var db = ServiceLocator.Get<MergeGridViewDataBase>();
            _background.color = db.mergeItemIdleColor;
            _scalable.DOScale(Vector3.one * db.mergeItemIdleScale, db.mergeItemScaleTime);
        }

        private void OnClicked()
        {
            if (_isChosen)
            {
                _isChosen = false;
                SetIdle();
            }
            else if(AllowChoose)
            {
                _isChosen = true;
                SetPicked();
            }
            OnStateChange?.Invoke();
        }

        private void OnEnable()
        {
            _btn.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveListener(OnClicked);
        }
    }
}