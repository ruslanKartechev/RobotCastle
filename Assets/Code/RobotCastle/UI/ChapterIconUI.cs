using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ChapterIconUI : MonoBehaviour
    {
        public void SetIcon(Sprite icon, bool isUnlocked)
        {
            if(_isAnimating)
                Stop();
            Reset();
            _current.image.sprite = _prev.image.sprite = icon;
            _current.SetLocked(!isUnlocked);
            _prev.SetLocked(!isUnlocked);
        }

        public void AnimateNext(Sprite icon, bool isUnlocked)
        {
            if(_isAnimating)
                Stop();
            Reset();
            _current.image.sprite = icon;
            _prev.SetLocked(_current.IsLocked);
            _current.SetLocked(!isUnlocked);
            AnimateInDir(-1);
        }

        public void AnimatePrev(Sprite icon, bool isUnlocked)
        {
            if(_isAnimating)
                Stop();
            Reset();
            _current.image.sprite = icon;
            _prev.SetLocked(_current.IsLocked);
            _current.SetLocked(!isUnlocked);
            AnimateInDir(1);
        }

        public void Stop()
        {
            _current.rect.DOKill();
            _prev.rect.DOKill();
        }

        public void SetUnlockRequirement(int chapterInd, int tierInd)
        {
            _current.unlockRequirement.text = $"Complete Chapter {chapterInd+1}\n" +
                                              $"<color=#FF1111>{TierNames[tierInd]}</color> Difficulty";
        }

        private static readonly List<string> TierNames = new (){ "Ease", "Normal", "Hard", "Very Hard", "King" };

        [SerializeField] private float _distance = 700;
        [SerializeField] private float _scale = .85f;
        [SerializeField] private float _animationTime = .85f;
        [SerializeField] private ImageData _current;
        [SerializeField] private ImageData _prev;
        private bool _isAnimating;


        // "+" means left to right (showing previous)
        private void AnimateInDir(int dir)
        {
            _isAnimating = true;
            var currentRect = _current.rect;
            var prevRect = _prev.rect;
            var pos = currentRect.anchoredPosition;
            pos.x = dir * -_distance;
            currentRect.anchoredPosition = pos;
            var lowScale = new Vector3(_scale, _scale, _scale);
            currentRect.localScale = lowScale;
            currentRect.DOAnchorPosX(0, _animationTime).SetEase(Ease.Linear);
            currentRect.DOScale(Vector3.one, _animationTime).SetEase(Ease.Linear);
            
            prevRect.anchoredPosition = Vector2.zero;
            prevRect.gameObject.SetActive(true);
            prevRect.DOAnchorPosX(dir * _distance, _animationTime).SetEase(Ease.Linear);
            prevRect.DOScale(lowScale, _animationTime).SetEase(Ease.Linear).OnComplete(OnAnimationEnd);
        }

        private void OnAnimationEnd()
        {
            _isAnimating = false;
            _prev.rect.gameObject.SetActive(false);
        }

        private void Reset()
        {
            _current.image.gameObject.SetActive(true);
            _prev.rect.gameObject.SetActive(false);
            _prev.image.sprite = _current.image.sprite;
            _prev.rect.anchoredPosition = _current.rect.anchoredPosition = Vector2.zero;
            _prev.rect.localScale = _current.rect.localScale = Vector3.one;
        }
        
        private void OnDisable()
        {
            if(_isAnimating)
                Stop();
        }

        [System.Serializable]
        private class ImageData
        {
            public Image image;
            public RectTransform rect;
            public TextMeshProUGUI unlockRequirement;
            public List<GameObject> lockedGo;
            public bool IsLocked { get; set; }

            public void SetLocked(bool locked)
            {
                IsLocked = locked;
                foreach (var go in lockedGo)
                    go.SetActive(locked);
            }
        }
    }
}