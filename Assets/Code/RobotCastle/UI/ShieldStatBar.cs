using System.Collections;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ShieldStatBar : MonoBehaviour
    {
        private const float startFillTime = .5f;
        
        [SerializeField] private GameObject _go;
        [SerializeField] private Image _fillImage;
        private Coroutine _filling;
        
        public void TrackUntilZero(IFloatGetter getter)
        {
            _go.SetActive(true);
            _filling = StartCoroutine(Filling(getter));
        }

        public void Hide()
        {
            _go.SetActive(false);
            if (_filling != null)
                StopCoroutine(_filling);
        }
        

        private IEnumerator Filling(IFloatGetter getter)
        {
            var elapsed = 0f;
            var t = 0f;
            var maxVal = getter.Get();
            while (elapsed < startFillTime)
            {
                _fillImage.fillAmount = Mathf.Lerp(0f, 1f, t);
                elapsed += Time.deltaTime;
                t = elapsed / startFillTime;
                yield return null;
            }
            _fillImage.fillAmount = 1f;
            while (getter.Get() > 0)
            {
                t = getter.Get() / maxVal;
                _fillImage.fillAmount = Mathf.Lerp(0f ,1f, t);
                yield return null;
            }
            _go.SetActive(false);
        }
    }
}