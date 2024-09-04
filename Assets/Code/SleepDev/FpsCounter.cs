using TMPro;
using UnityEngine;

namespace SleepDev
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField, Range(0f,1f)] private float _updatePeriod = 1f;

        private float _elapsed = 0f;
        private int _framesCount;

        public void Begin()
        {
            enabled = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        
        private void Update()
        {
            _elapsed += Time.unscaledDeltaTime;
            _framesCount++;   
            
            if (_elapsed >= _updatePeriod)
            {
                var fps = _framesCount / _elapsed;
                // _text.text = $"{(int)(1f / Time.unscaledDeltaTime)}";
                _text.text = $"{(int)(fps)}";
                _elapsed = 0f;
                _framesCount = 0;
            }
        }
    }
}