using System.Collections;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class XpAddAnimation : MonoBehaviour
    {
        [SerializeField] private float _animationTime;
        [SerializeField] private Image _fillImage;
        private Coroutine _working;
        
        public void Start()
        {
            _fillImage.SetAlpha(0f);
        }

        public void Animate(float maxVal)
        {
            _fillImage.fillAmount = maxVal;
            Stop();
            _working = StartCoroutine(Working());
        }

        public void Stop()
        {
            if (_working != null)
                StopCoroutine(_working);
            _fillImage.SetAlpha(0f);
        }

        private IEnumerator Working()
        {
            while (true)
            {
                yield return Animating(0f, 1f, _animationTime);
                yield return Animating(1f, 0f, _animationTime);
            }
        }

        private IEnumerator Animating(float from, float to, float time)
        {
            var elapsed = 0f;
            var t = 0f;
            while (t < 1f)
            {
                var a  = Mathf.Lerp(from, to, t);
                _fillImage.SetAlpha(a);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
        }
        
    }
}