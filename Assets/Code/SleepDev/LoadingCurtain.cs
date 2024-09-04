using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    [DefaultExecutionOrder(-100)]
    public class LoadingCurtain : MonoBehaviour
    {
        public static void Open(Action onEnd) => _instance.OpenCurtains(onEnd);
        public static void Close(Action onEnd) => _instance.CloseCurtains(onEnd);
        public static void CloseNow() => _instance.CloseCurtainsNow();
        public static void OpenNow() => _instance.OpenCurtainsNow();
        
        private static LoadingCurtain _instance;
        
        [SerializeField] private float _openTime;
        [SerializeField] private float _closeTime;
        [SerializeField] private List<Curtain> _curtains;
        private Coroutine _moving;


        public void Init()
        {
            _instance = this;
        }

        private void OpenCurtainsNow()
        {
            foreach (var curtain in _curtains)
            {
                curtain.Hide();
                curtain.Opening(1);
            }
        }        
        
        private void CloseCurtainsNow()
        {
            foreach (var curtain in _curtains)
            {
                curtain.Show();
                curtain.Closing(1);
            }
        }    
        
        private void OpenCurtains(Action onEnd)
        {
            if (!gameObject.activeInHierarchy)
                return;
            Stop();
            _moving = StartCoroutine(Opening(onEnd));
        }

        private void CloseCurtains(Action onEnd)
        {
            gameObject.SetActive(true);
            Stop();
            _moving = StartCoroutine(Closing(onEnd));
        }

        private void Stop()
        {
            if(_moving != null)
                StopCoroutine(_moving);
        }
        
        private IEnumerator Opening(Action onEnd)
        {
            var elapsed = 0f;
            while (elapsed <= _openTime)
            {
                foreach (var curtain in _curtains)
                    curtain.Opening(elapsed / _openTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            foreach (var curtain in _curtains)
            {
                curtain.Opening(1);
                curtain.Hide();
            }
            onEnd?.Invoke();
        }
        
        private IEnumerator Closing(Action onEnd)
        {
            foreach (var curtain in _curtains)
                curtain.Show();
            var elapsed = 0f;
            while (elapsed <= _closeTime)
            {
                foreach (var curtain in _curtains)
                    curtain.Closing(elapsed / _closeTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach (var curtain in _curtains)
                curtain.Closing(1);
            onEnd?.Invoke();
        }

        
        [System.Serializable]
        public class Curtain
        {
            public RectTransform movable;
            public Vector2 openPos;
            public Vector2 closedPos;

            public void Opening(float t)
            {
                movable.anchoredPosition = Vector2.Lerp(closedPos, openPos, t);
            }

            public void Closing(float t)
            {
                movable.anchoredPosition = Vector2.Lerp(openPos,closedPos, t);
            }

            public void Hide()
            {
                movable.gameObject.SetActive(false);
            }

            public void Show()
            {
                movable.gameObject.SetActive(true);
            }
        }
        
    }
    
    
}