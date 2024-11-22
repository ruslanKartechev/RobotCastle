using System;
using System.Collections;
using RobotCastle.Core;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class ShopTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        private Coroutine _working;
        
        public DateTimeData EndTime { get; set; }
        public Action TimerEndCallback { get; set; }

        public void Init(DateTimeData endTime, Action endCallback)
        {
            EndTime = endTime;
            TimerEndCallback = endCallback;
            SetTime(endTime.GetDateTime() - DateTime.Now);
        }
        
        public void InitMinutes(DateTimeData endTime, Action endCallback)
        {
            EndTime = endTime;
            TimerEndCallback = endCallback;
            SetTimeMinutes(endTime.GetDateTime() - DateTime.Now);
        }
        
        public void Begin()
        {
            Stop();
            _working = StartCoroutine((IEnumerator)Counting());
        }

        public void BeginMinutes()
        {
            Stop();
            _working = StartCoroutine(CountingMinutes());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private IEnumerator CountingMinutes()
        {
            var endTime = EndTime.GetDateTime();
            while (true)
            {
                var nowTime = DateTime.Now;
                if (nowTime >= endTime)
                {
                    _timerText.text = "00:00";
                    TimerEndCallback?.Invoke();
                    yield break;
                }

                var delta = (endTime - nowTime);
                SetTimeMinutes(delta);
                yield return null;
            }
        }

        private IEnumerator Counting()
        {
            var endTime = EndTime.GetDateTime();
            while (true)
            {
                var nowTime = DateTime.Now;
                if (nowTime >= endTime)
                {
                    _timerText.text = "00h: 00m: 00s";
                    TimerEndCallback?.Invoke();
                    yield break;
                }

                var delta = (endTime - nowTime);
                SetTime(delta);
                yield return null;
            }
        }

        private void SetTime(TimeSpan timeSpan)
        {
            _timerText.text = $"{timeSpan.Hours:00}h: {timeSpan.Minutes:00}m: {timeSpan.Seconds:00}s";
        }
        
        private void SetTimeMinutes(TimeSpan timeSpan)
        {
            _timerText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
        
    }
}