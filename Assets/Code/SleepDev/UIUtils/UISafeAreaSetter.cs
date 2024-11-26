using System;
using UnityEngine;

namespace SleepDev
{
    public class UISafeAreaSetter : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        private Vector2 _size;
 
        private void OnEnable()
        {
            Adjust();
        }

        #if UNITY_EDITOR
        private void Update()
        {
            var size =new Vector2(Screen.width, Screen.height);
            if(size != _size)
                Adjust();
        }
        #endif

        public void Adjust()
        {
            _size = new Vector2(Screen.width, Screen.height);
            var safeAreaRect = Screen.safeArea;
            var scaleRatio = _rect.rect.width / Screen.width;
            var scaleRatioVert = _rect.rect.height / Screen.height;
            
            var left = safeAreaRect.xMin * scaleRatio;
            var right = -( _size.x - safeAreaRect.xMax ) * scaleRatio;
            
            var top = -safeAreaRect.yMin * scaleRatioVert;
            var bottom = (_size.y - safeAreaRect.yMax) * scaleRatioVert;
            
            _rect.offsetMin = new Vector2( left, -top);
            _rect.offsetMax = new Vector2( right, -bottom );
            // CLog.Log($"offset min: {_rect.offsetMin}, max: {_rect.offsetMax}. SCALE: {scaleRatio}");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}