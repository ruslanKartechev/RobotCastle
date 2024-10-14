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
        private Vector2 _sizeNorm;
        private Vector2 _sizeBig;
        
        private void Awake()
        {
            _sizeNorm.x = element.preferredWidth;
            _sizeNorm.y = element.minHeight;
            
            _sizeBig.x = element.preferredWidth * 1.15f;
            _sizeBig.y = element.minHeight * 1.12f;
        }

        public void Animate(bool up, Color color, Sprite sprite, float time)
        {
            background.sprite = sprite;
            if(_animating != null) StopCoroutine(_animating);
            _animating = StartCoroutine(Scaling(up ? _sizeBig : _sizeNorm, time, color));
        }
        
        private IEnumerator Scaling(Vector2 size2, float time, Color endColor)
        {
            var c1 = background.color;
            // var size1 = new Vector2(element.preferredWidth, element.preferredHeight);
            var size1 = new Vector2(element.preferredWidth, element.minHeight);
            
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                background.color = Color.Lerp(c1, endColor, t);
                element.preferredWidth = Mathf.Lerp(size1.x, size2.x, t);
                element.minHeight = Mathf.Lerp(size1.y, size2.y, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            element.preferredWidth = size2.x;
            element.minHeight = size2.y;
            background.color = endColor;
        }
        
    }
}