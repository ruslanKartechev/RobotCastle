using System.Collections;
using RobotCastle.Battling;
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
        
        public void TrackUntilZero(HeroStatsManager stats)
        {
            _go.SetActive(true);
            _filling = StartCoroutine(Filling(stats));
            
        }

        public void Hide()
        {
            _go.SetActive(false);
            if (_filling != null)
                StopCoroutine(_filling);
        }
        

        private IEnumerator Filling(HeroStatsManager stats)
        {
            var elapsed = 0f;
            var t = 0f;
            var max = stats.Shield / stats.HealthMax.Get();
            while (elapsed < startFillTime)
            {
                _fillImage.fillAmount = Mathf.Lerp(0f, max, t);
                elapsed += Time.deltaTime;
                t = elapsed / startFillTime;
                yield return null;
            }
            _fillImage.fillAmount = max;
            while (stats.Shield > 0)
            {
                t = stats.Shield / stats.HealthMax.Get();
                _fillImage.fillAmount = t;
                yield return null;
            }
            _go.SetActive(false);
        }
    }
}