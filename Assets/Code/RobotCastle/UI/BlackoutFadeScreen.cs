using System;
using System.Collections;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class BlackoutFadeScreen : MonoBehaviour
    {
        public string Id { get; set; }
        
        [SerializeField] private float _durationIn = .3f;
        [SerializeField] private float _durationOut = .21f;
        [SerializeField] private float _maxAlpha = 1f;
        [SerializeField] private Color _color1;
        [SerializeField] private Color _color2;
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _goToDisable;
        private Coroutine _working;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_image == null)
            {
                _image = gameObject.GetComponent<Image>();
                if (_image != null)
                {
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
#endif

        
        public void FadeInWithId(string id)
        {
            Id = id;
            FadeIn();
        }
        
        public void FadeOut(string id)
        {
            Id = id;
            gameObject.SetActive(true);
            if(_working != null)
                StopCoroutine(_working);
            _working = StartCoroutine(FadingOut());
        }

        public void FadeIn()
        {
            gameObject.SetActive(true);
            if(_working != null)
                StopCoroutine(_working);
            _working = StartCoroutine(FadingIn());
        }

        public void FadeOut()
        {
            gameObject.SetActive(true);
            if(_working != null)
                StopCoroutine(_working);
            _working = StartCoroutine(FadingOut());
        }

        private IEnumerator FadingIn()
        {
            _image.enabled = true;
            _image.SetAlpha(_maxAlpha);
            yield return AlphaChange(_maxAlpha, 0f, _durationIn);
            // yield return ColorChange(_color1, _color2, _durationOut);

            _image.enabled = false;
        }

        private IEnumerator FadingOut()
        {
            _image.enabled = true;
            yield return AlphaChange(0f, _maxAlpha, _durationOut);
            // yield return ColorChange(_color2, _color1, _durationOut);
            
            ServiceLocator.Get<IUIManager>().OnClosed(Id);
            if (_goToDisable == null)
                _goToDisable = transform.parent.gameObject;
            _goToDisable.gameObject.SetActive(false);
        }
        
        private IEnumerator ColorChange(Color one, Color two, float time)
        {
            var elapsed = 0f;
            while (elapsed < time)
            {
                var color = Color.Lerp(one, two, elapsed / time);
                _image.color = color;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _image.color = two;
        }
        
        private IEnumerator AlphaChange(float from, float to, float time)
        {
            var elapsed = 0f;
            while (elapsed < time)
            {
                var a = Mathf.Lerp(from, to, elapsed / time);
                _image.SetAlpha(a);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _image.SetAlpha(to);
        }
        
    }
}