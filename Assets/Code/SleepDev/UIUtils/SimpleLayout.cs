using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class SimpleLayout : LayoutBase
    {
        [SerializeField] private List<ElementsGroup> _groups;
        [SerializeField] private AnimationCurve _commonCurve;
        [SerializeField] private List<GameObject> _hideTargets;
        private Coroutine _moving;
        
        public override void SetLayout(Action onDone, bool animated = true)
        {
            foreach (var go in _hideTargets)
                go.SetActive(false);
            if (animated)
            {
                StopMoving();
                _moving = StartCoroutine(Moving(onDone));
            }
            else
            {
                foreach (var group in _groups)
                {
                    foreach (var element in group.elements)
                        element.Set();       
                }
            }
        }

        private void StopMoving()
        {
            if( _moving != null)
                StopCoroutine(_moving);
        }

        private IEnumerator Moving(Action onDone)
        {
            foreach (var group in _groups)
            {
                if (group.delay > 0)
                    yield return new WaitForSeconds(group.delay);
                
                var elements = group.elements;
                foreach (var element in elements)
                    element.SavePosition();
                var elapsed = 0f;
                var time = _setTime;
                
                while (elapsed <= time)
                {
                    var fract = elapsed / time;
                    foreach (var element in elements)
                    {
                        var t = fract * element.SpeedMod;
                        element.Move(t);
                    }
                    elapsed += Time.deltaTime * _commonCurve.Evaluate(fract);
                    yield return null;
                }
                
                foreach (var element in elements)
                    element.Full();
            }
            onDone?.Invoke();
        }


        
        
        
        [System.Serializable]
        public class ElementsGroup
        {
            public float delay;
            public List<MovableElement> elements;
        }
        
        

        [System.Serializable]
        public class MovableElement
        {
            [SerializeField] private float _speedModifier = 1;
            [SerializeField] private RectTransform _rect;
            [SerializeField] private bool _disableOnFull;
            [Header("Start Position")] 
            [SerializeField] private bool _useFromPosition;
            [SerializeField] private Vector2 _startPosition;
            [Header("End Position")]
            [SerializeField] private Vector2 _endPosition;
            [Header("Anchors")]
            [SerializeField] private Vector2 _anchorsMin;
            [SerializeField] private Vector2 _anchorsMax;
            
            public float SpeedMod => _speedModifier;
            private Vector2 _savedPosition;

            public void SavePosition()
            {
                if (_useFromPosition)
                    _rect.anchoredPosition = _startPosition;
                _savedPosition = _rect.anchoredPosition;
                _rect.gameObject.SetActive(true);
                _rect.anchorMin = _anchorsMin;
                _rect.anchorMax = _anchorsMax;
            }

            public void Set()
            {
                _rect.anchorMin = _anchorsMin;
                _rect.anchorMax = _anchorsMax;
                _rect.anchoredPosition = _endPosition;
                if(_disableOnFull)
                    _rect.gameObject.SetActive(false);
                else
                    _rect.gameObject.SetActive(true);
            }

            public void Full()
            {
                Move(1);
                if(_disableOnFull)
                    _rect.gameObject.SetActive(false);
            }
            
            public void Move(float t)
            {
                _rect.anchoredPosition = Vector2.Lerp(_savedPosition, _endPosition, t);
            }
        }
    }
}