using SleepDev;
using UnityEngine;

namespace RobotCastle.UI
{
    public class UIRectToScreenFitter : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        
        public RectTransform Rect => _rect; 

        public void SetScreenPos(Vector3 pos, float addedWidth = 0, float addedHeight = 0)
        {
            pos.z = 0f;
            var width = _rect.sizeDelta.x + addedWidth;
            var height = _rect.sizeDelta.y + addedHeight;
            
            var offsetX = new Vector2((-_rect.pivot.x) * width, (1 - _rect.pivot.x) * width);
            var offsetY = new Vector2((-_rect.pivot.y) * height, (1 - _rect.pivot.y) * height);

            var rightOverflow = (pos.x + offsetX.y) - Screen.width;
            if (rightOverflow > 0)
                pos.x -= rightOverflow;
            var leftOverflow = (pos.x + offsetX.x);
            if (leftOverflow < 0)
                pos.x = Mathf.Abs(offsetX.x);
            
            var topOverflow = (pos.y + offsetY.y) - Screen.height;
            if (topOverflow > 0)
                pos.y -= topOverflow;
            var botOverflow = (pos.y + offsetY.x);
            if (botOverflow < 0)
                pos.y = Mathf.Abs(offsetY.x);
            
            _rect.position = pos;
        }
    }
}