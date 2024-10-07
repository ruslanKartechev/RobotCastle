using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SleepDev
{
    public class LeftRightScroller : MonoBehaviour
    {
        public event Action OnNewItemChosen;
          
        public List<object> assignedObjects { get; } = new(10);
            
        public int chosenIndex => _index;

        public object currentObject
        {
            get
            {
                if (_index >= assignedObjects.Count)
                {
                    CLog.LogError("Chosen index > assigned objects count");
                    return default;
                }
                return assignedObjects[_index];
            }
        }

        public object nextRightObject
        {
            get
            {
                var next = CorrectCircularIndex(_index + 1);
                return assignedObjects[next];
            }
        }
          
        public object nextLeftObject
        {
            get
            {
                var next = CorrectCircularIndex(_index - 1);
                return assignedObjects[next];
            }
        }
          
        public bool inputAllowed { get; set; } = true;

        [SerializeField] private float _animationTime;
        [SerializeField] private Vector3 _sidesScale; 
        [SerializeField] private Vector3 _centerScale;
        [SerializeField] private List<RectTransform> _rects;
        [SerializeField] private List<ScrollElement> _elements;
        [SerializeField] private RectTransform _activeAreaParent;
        [SerializeField] private RectTransform _backgroundParent;
        [SerializeField] private Button _leftBtn;
        [SerializeField] private Button _rightBtn;
        [SerializeField] private RectTransform _centerPoint;
        [SerializeField] private RectTransform _rightPoint;
        [SerializeField] private RectTransform _leftPoint;
        [SerializeField] private int _index;
        private Coroutine _animating;
        private bool _isAnimating;
        

        private void OnEnable()
        {
            _isAnimating = false;
            _leftBtn.onClick.AddListener(Left);
            _rightBtn.onClick.AddListener(Right);
            _index = CorrectCircularIndex(_index);
            SetPositionsForCurrentIndex();
        }

        private void OnDisable()
        {
            _isAnimating = false;
            _leftBtn.onClick.RemoveListener(Left);
            _rightBtn.onClick.RemoveListener(Right);
        }

        [ContextMenu("SetPositionsForCurrentIndex")]
        private void SetPositionsForCurrentIndex()
        {
            if (_rects.Count < 3)
                return;
            var centerInd = CorrectCircularIndex(_index);
            var rightInd = CorrectCircularIndex(centerInd + 1);
            var leftInd = CorrectCircularIndex(centerInd - 1);
            for (var i = 0; i < _rects.Count; i++)
            {
                var rr = _rects[i];
                rr.gameObject.SetActive(false);
                _elements[i].Off();
            }
            _elements[centerInd].On();

            _rects[centerInd].gameObject.SetActive(true);
            _rects[rightInd].gameObject.SetActive(true);
            _rects[leftInd].gameObject.SetActive(true);
            
            _rects[centerInd].anchoredPosition = _centerPoint.anchoredPosition;
            _rects[rightInd].anchoredPosition = _rightPoint.anchoredPosition;
            _rects[leftInd].anchoredPosition = _leftPoint.anchoredPosition;
            
            _rects[leftInd].localScale = _rects[rightInd].localScale = _sidesScale;
            _rects[centerInd].localScale = _centerScale;

            _rects[centerInd].parent = _activeAreaParent;
            _rects[centerInd].SetAsLastSibling();
            _rects[rightInd].parent = _rects[leftInd].parent = _backgroundParent;
            _rects[leftInd].SetAsLastSibling();
            _rects[rightInd].SetAsLastSibling();

            OnNewItemChosen?.Invoke();
        }

        private void Left()
        {
            if (_isAnimating)
                return;
            if (_rects.Count < 3)
                return;
            var currentCenterInd = CorrectCircularIndex(_index);
            var currentRightInd = CorrectCircularIndex(currentCenterInd + 1);
            var newCenterInd = CorrectCircularIndex(currentCenterInd - 1);
            var newLeftInd = CorrectCircularIndex(currentCenterInd - 2);

            var center = _rects[currentCenterInd];
            var newCenter = _rects[newCenterInd];
            var right = _rects[currentRightInd];
            var newLeft = _rects[newLeftInd];
            center.gameObject.SetActive(true);
            newCenter.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
            newLeft.gameObject.SetActive(true);
            _elements[currentCenterInd].Close(true);
            _elements[newCenterInd].Show(true);
            
            center.anchoredPosition = _centerPoint.anchoredPosition;
            right.anchoredPosition = _rightPoint.anchoredPosition;
            newCenter.anchoredPosition = newLeft.anchoredPosition = _leftPoint.anchoredPosition;

            center.localScale = _centerScale;
            right.localScale = newLeft.localScale = newCenter.localScale = _sidesScale;
            
            newCenter.SetParent(_activeAreaParent);
            center.SetParent(_backgroundParent);
            right.SetParent(_backgroundParent);
            newLeft.SetParent(_backgroundParent);
            
            newCenter.SetAsLastSibling();
            right.SetAsLastSibling();
            newLeft.SetAsLastSibling();
            center.SetAsLastSibling();

            _index = newCenterInd;
            if(_animating != null) StopCoroutine(_animating);
            _animating = StartCoroutine(AnimatingLeftToRight(center, newCenter, right, newLeft));
            
            OnNewItemChosen?.Invoke();
        }

        private void Right()
        {
            if (_isAnimating)
                return;
            if (_rects.Count < 3)
                return;
            var currentCenterInd = CorrectCircularIndex(_index);

            var currentLeftInd = CorrectCircularIndex(currentCenterInd - 1);
            var newCenterInd = CorrectCircularIndex(currentCenterInd + 1);
            var newRightInd = CorrectCircularIndex(currentCenterInd + 2);
            _elements[currentCenterInd].Close(false);
            _elements[newCenterInd].Show(false);
            
            var center = _rects[currentCenterInd];
            var newCenter = _rects[newCenterInd];
            var left = _rects[currentLeftInd];
            var newRight = _rects[newRightInd];
            center.gameObject.SetActive(true);
            newCenter.gameObject.SetActive(true);
            left.gameObject.SetActive(true);
            newRight.gameObject.SetActive(true);
            
            center.anchoredPosition = _centerPoint.anchoredPosition;
            left.anchoredPosition = _leftPoint.anchoredPosition;
            newCenter.anchoredPosition = newRight.anchoredPosition = _rightPoint.anchoredPosition;

            center.localScale = _centerScale;
            left.localScale = newRight.localScale = newCenter.localScale = _sidesScale;
            
            newCenter.SetParent(_activeAreaParent);
            center.SetParent(_backgroundParent);
            left.SetParent(_backgroundParent);
            newRight.SetParent(_backgroundParent);

            newCenter.SetAsLastSibling();
            left.SetAsLastSibling();
            newRight.SetAsLastSibling();
            center.SetAsLastSibling();
            
            _index = newCenterInd;
            if(_animating != null) StopCoroutine(_animating);
            _animating = StartCoroutine(AnimatingRightToLeft(center, newCenter, left, newRight));
        }

        private IEnumerator AnimatingRightToLeft(RectTransform center, RectTransform newCenter,
            RectTransform left, RectTransform newRight)
        {
            var elapsed = 0f;
            var time = _animationTime;
            var t = 0f;
            while (t <= 1)
            {
                center.anchoredPosition = Vector2.Lerp(_centerPoint.anchoredPosition, _leftPoint.anchoredPosition, t);
                newCenter.anchoredPosition = Vector2.Lerp(_rightPoint.anchoredPosition, _centerPoint.anchoredPosition, t);
                center.localScale = Vector3.Lerp(_centerScale, _sidesScale, t);
                newCenter.localScale = Vector3.Lerp(_sidesScale, _centerScale, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            left.gameObject.SetActive(false);
            OnAnimationEnded();
        }
        
        
        private IEnumerator AnimatingLeftToRight(RectTransform center, RectTransform newCenter,
            RectTransform right, RectTransform newLeft)
        {
            var elapsed = 0f;
            var time = _animationTime;
            var t = 0f;
            while (t <= 1)
            {
                center.anchoredPosition = Vector2.Lerp(_centerPoint.anchoredPosition, _rightPoint.anchoredPosition, t);
                newCenter.anchoredPosition = Vector2.Lerp(_leftPoint.anchoredPosition, _centerPoint.anchoredPosition, t);
                center.localScale = Vector3.Lerp(_centerScale, _sidesScale, t);
                newCenter.localScale = Vector3.Lerp(_sidesScale, _centerScale, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            right.gameObject.SetActive(false);
            OnAnimationEnded();
        }

        private void OnAnimationEnded()
        {
            OnNewItemChosen?.Invoke();
            _animating = null;
            _isAnimating = false;
        }
        
        private int CorrectCircularIndex(int index)
        {
            if (index < 0)
                return index + _rects.Count;
            if (index >= _rects.Count)
                return index - _rects.Count;
            return index;
        }
    }
}