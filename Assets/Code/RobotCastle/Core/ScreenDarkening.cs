using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Core
{
    public class ScreenDarkening : MonoBehaviour
    {
        public static void Animate(Action onDark, Action onEnd = null)
            => _inst.StartCoroutine(_inst.Animating(onDark, onEnd));

        [SerializeField] private float _timeIn;
        [SerializeField] private float _timeOut;
        [SerializeField] private AnimationCurve _curveIn;
        [SerializeField] private AnimationCurve _curveOut;
        [SerializeField] private List<Image> _images;
        private static ScreenDarkening _inst;
        
        private void Awake()
        {
            _inst = this;
            transform.parent = null;
            foreach (var im in _images)
                im.gameObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator Animating(Action onDark, Action onEnd)
        {
            foreach (var im in _images)
                im.gameObject.SetActive(true);
            var elapsed = 0f;
            while (elapsed < _timeIn)
            {
                var t = elapsed / _timeIn;
                var a = Mathf.Lerp(0f, 1f, t);
                foreach (var im in _images)
                    im.SetAlpha(a);
                elapsed += Time.deltaTime * _curveIn.Evaluate(t);
                yield return null;
            }
            onDark?.Invoke();
            elapsed = 0f;
            while (elapsed < _timeOut)
            {
                var t = elapsed / _timeOut;
                var a = Mathf.Lerp(1f, 0f, t);
                foreach (var im in _images)
                    im.SetAlpha(a);
                elapsed += Time.deltaTime * _curveOut.Evaluate(t);
                yield return null;
            }
            foreach (var im in _images)
                im.gameObject.SetActive(false);
            onEnd?.Invoke();
        }

    }
}