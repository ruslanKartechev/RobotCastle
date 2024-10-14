using System.Collections;
using System.Globalization;
using RobotCastle.Battling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SleepDev;

namespace RobotCastle.UI
{
    public class BarTrackerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Image _fillImageBack;
        
        private Coroutine _changing;
        private Stat _stat;
        private Stat _statMax;
        private int _prevVal;
        private int _prevMax;
        private float _prevT;
        
        private const float _fillTime = 0.4f;
        
        public void SetVal(float val)
        {
            _fillImage.fillAmount = val;
        }

        public void DisplayStats(Stat stat, Stat statMax)
        {
            _prevVal = (int)stat.Get();
            _prevMax = (int)statMax.Get();
            _prevT = (float)(_prevVal) / _prevMax;
            
            if(_text != null)
                _text.text = _prevVal.ToString(CultureInfo.InvariantCulture);
            
            _fillImage.fillAmount = _prevT;
            if(_fillImageBack != null)
                _fillImageBack.fillAmount = _prevT;
        }
        
        public void AssignStats(Stat stat, Stat statMax)
        {
            _stat = stat;
            _statMax = statMax;

            if (_stat != null && _statMax != null)
            {
                DisplayStats(_stat, _statMax);
                // CLog.LogRed($"[Assign] {_prevVal}, {_prevMax}, {_prevT}");
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        }

        protected void OnEnable()
        {
            if (_stat == null)
                enabled = false;
        }
        
        [ContextMenu("Debug")]
        protected void Debug()
        {
            CLog.LogRed($"Enabled: {enabled}. Stat: {_stat.Get()}. Max: {_statMax.Get()}. T: {_stat.Get() / _statMax.Get()}");
        }

        protected void Update()
        {
            var val = (int)_stat.Get();
            var max = (int)_statMax.Get();
            var t = (float)val / max;
            if (val != _prevVal || max != _prevMax)
            {
                if (_text != null)
                    _text.text = val.ToString();
                
                if (t < _prevT)
                {
                    if (_fillImageBack != null)
                    {
                        if(_changing != null)
                            StopCoroutine(_changing);
                        _changing = StartCoroutine(Changing(_prevT, t));      
                    }
                    else
                    {
                        _fillImage.fillAmount = t;                        
                    }
                }
                else
                {
                    if (_fillImageBack != null)
                    {
                        if(_changing != null)
                            StopCoroutine(_changing);
                        _fillImageBack.fillAmount = t;
                    }
                    _fillImage.fillAmount = t;
                }
                    
                _prevMax = max;
                _prevVal = val;
                _prevT = t;
            }
            
        }

        private IEnumerator Changing(float t1, float t2)
        {
            _fillImage.fillAmount = t2;
            _fillImageBack.fillAmount = t1;
            var elapsed = 0f;
            while (elapsed < _fillTime)
            {
                _fillImageBack.fillAmount = Mathf.Lerp(t1, t2, elapsed / _fillTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _fillImageBack.fillAmount = t2;
        }
    }
}