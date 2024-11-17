using UnityEngine;

namespace SleepDev
{
    public class UISafeAreaSetter : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect; 
 
        private void OnEnable()
        {
            Adjust();
        }

        public void Adjust()
        {
            var safeAreaRect = Screen.safeArea;
            var scaleRatio = _rect.rect.width / Screen.width;
            var left = safeAreaRect.xMin * scaleRatio;
            var right = -( Screen.width - safeAreaRect.xMax ) * scaleRatio;
            var top = -safeAreaRect.yMin * scaleRatio;
            _rect.offsetMin = new Vector2( left, 0 );
            _rect.offsetMax = new Vector2( right, top );
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