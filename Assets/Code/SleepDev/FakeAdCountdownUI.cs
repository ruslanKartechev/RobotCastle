using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace SleepDev
{
    public class FakeAdCountdownUI : MonoBehaviour
    {
        private const string PathToPrefab = "prefabs/ui/fake_ads_countdown_ui";
        private static FakeAdCountdownUI _inst;
        
        public static FakeAdCountdownUI Get()
        {
            if (_inst != null && _inst.gameObject != null)
                return _inst;
            _inst = Instantiate(Resources.Load<FakeAdCountdownUI>(PathToPrefab));
            return _inst;
        }
        
        
        [SerializeField] private TextMeshProUGUI _textPrompt;
        [SerializeField] private TextMeshProUGUI _textCountdown;
        [SerializeField] private float _delay = 3f;
        private Action<bool> _callback;

        public float delay
        {
            get => _delay;
            set => _delay = value;
        }

        public Action<bool> callback
        {
            get => _callback;
            set => _callback = value;
        }


        public void Show(string prompt, Action<bool> callback)
        {
            Time.timeScale = 1f;
            _textPrompt.text = prompt;
            gameObject.SetActive(true);
            _callback = callback;
            StartCoroutine(Working());
        }

        private void OnEnd()
        {
            gameObject.SetActive(false);
            _callback?.Invoke(true);
        }
        
        private IEnumerator Working()
        {
            var time = _delay;
            while (time > 0)
            {
                _textCountdown.text = $"{time:N1}";
                time -= Time.unscaledDeltaTime;
                yield return null;
            }
            _textCountdown.text = "0";
            OnEnd();
        }
    }
}