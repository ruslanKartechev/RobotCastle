using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class TabBtn : MonoBehaviour
    {
        public RectTransform rect;
        public LayoutElement element;
        public Image background;
        private Coroutine _animating;
        
        public void Animate(Vector2 size, Color color, Sprite sprite, float time)
        {
            background.sprite = sprite;
            if(_animating != null) StopCoroutine(_animating);
            _animating = StartCoroutine(Scaling(size, time, color));
        }
        
        private IEnumerator Scaling(Vector2 endSize, float time, Color endColor)
        {
            var c1 = background.color;
            // var size1 = new Vector2(element.preferredWidth, element.preferredHeight);
            var s1 = rect.localScale;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                rect.localScale =Vector3.Lerp(s1, endSize, t);
                // var size = Vector2.Lerp(size1, endSize, t);
                // element.preferredWidth = size.x;
                // element.preferredHeight = size.y;
                background.color = Color.Lerp(c1, endColor, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            rect.localScale = endSize;
            // element.preferredWidth = endSize.x;
            // element.preferredHeight = endSize.y;
            background.color = endColor;

        }
    }
}