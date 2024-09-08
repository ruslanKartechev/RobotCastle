using System;
using DG.Tweening;
using UnityEngine;

namespace RobotCastle.Core
{
    public class ScaleWorldButton : MonoBehaviour, IWorldButton
    {
        [SerializeField] private Transform _scalable;
        [SerializeField] private Vector3 _scaleDown;
        [SerializeField] private Vector3 _scaleUp;
        [SerializeField] private Vector2 _scaleTime;
        [SerializeField] private Ease _scaleUpEase;
        
        public event Action OnClicked;
        
        public void OnDown()
        {
            _scalable.DOScale(_scaleDown, _scaleTime.x);
        }

        public void OnUp()
        {
            _scalable.DOScale(_scaleUp, _scaleTime.y).SetEase(_scaleUpEase);
            OnClicked?.Invoke();
        }
    }
}