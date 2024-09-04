using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class HorizontalFitter : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> _rects;
        [SerializeField] private float _spacing;
        [SerializeField] private float _size;
        
        public List<RectTransform> rects
        {
            get => _rects;
            set => _rects = value;
        }

        public float spacing
        {
            get => _spacing;
            set => _spacing = value;
        }

        public float size
        {
            get => _size;
            set => _size = value;
        }
        
        [ContextMenu("FitAll")]
        public void FitAll()
        {
            FitAsTheSame(_rects);
        }
        
        [ContextMenu("FitActive")]
        public void FitActive()
        {
            var active = new List<RectTransform>(_rects.Count);
            foreach (var rr in _rects)
            {
                if(rr.gameObject.activeSelf)
                    active.Add(rr);
            }
            FitAsTheSame(active);
        }

        public void Fit(List<RectTransform> rects)
        {
            var count = rects.Count;
            var totalLength = 0f;
            foreach (var rr in rects)
                totalLength += rr.rect.width * .5f;
            totalLength += (count - 1) * _spacing;
        }

        public void FitAsTheSame(List<RectTransform> rects)
        {
            var count = rects.Count;
            if (count == 0)
                return;
            var leftStepsCount = 0f;
            leftStepsCount = (count - 1) / 2f;
            var start = -(leftStepsCount * _size + (count - 1) * _spacing);
            var x = start;
            for (var i = 0; i < count; i++)
            {
                var pos = rects[i].anchoredPosition;
                pos.x = x;
                x += _size + _spacing;
                rects[i].anchoredPosition = pos;
            }
            
        }
    }
}