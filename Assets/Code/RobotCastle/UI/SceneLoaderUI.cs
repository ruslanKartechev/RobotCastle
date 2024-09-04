using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class SceneLoaderUI : MonoBehaviour
    {
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private Image _fillImage;
        [SerializeField] private float _fillTime;
        [SerializeField] private TextMeshProUGUI _percentText;
        private Coroutine _working;
        
        
        private void Start()
        {
            if(_autoStart)
                Begin();
        }

        public void Begin()
        {
            if(_working != null)
                StopCoroutine(_working);
            if (_percentText != null)
                _working = StartCoroutine(LoadingWithText());
            else
                _working = StartCoroutine(LoadingNoText());
        }

        private IEnumerator LoadingNoText()
        {
            var t = 0f;
            var elapsed = 0f;
            var time = _fillTime;
            while (t < 99)
            {
                var lt = elapsed / time;
                t = Mathf.RoundToInt(lt * 100f);
                _fillImage.fillAmount = lt;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        private IEnumerator LoadingWithText()
        {
            var t = 0f;
            var elapsed = 0f;
            var time = _fillTime;
            while (t < 99)
            {
                var lt = elapsed / time;
                t = Mathf.RoundToInt(lt * 100f);
                _fillImage.fillAmount = lt;
                _percentText.text = $"{t}%";
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
    }
}