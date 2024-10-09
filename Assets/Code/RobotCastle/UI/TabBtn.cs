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
        private const float scaleTime = .4f;
        private Coroutine _animating;
        
        public void Animate(float width, Color color, Sprite sprite)
        {
            background.sprite = sprite;
            if(_animating != null) StopCoroutine(_animating);
            _animating = StartCoroutine(Scaling(width, scaleTime, color));
        }
        
        private IEnumerator Scaling(float endVal, float time, Color endColor)
        {
            var c1 = background.color;
            var w1 = element.preferredWidth;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                element.preferredWidth = Mathf.Lerp(w1, endVal, t);
                background.color = Color.Lerp(c1, endColor, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            element.preferredWidth = endVal;
            background.color = endColor;

        }
    }
}