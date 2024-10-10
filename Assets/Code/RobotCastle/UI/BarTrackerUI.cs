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
        private Stat _stat;
        private Stat _statMax;

        public void SetVal(float val)
        {
            _fillImage.fillAmount = val;
        }

        public void DisplayStats(Stat stat, Stat statMax)
        {
            if(_text != null)
                _text.text = stat.Get().ToString(CultureInfo.InvariantCulture);
            var t = stat.Get() / statMax.Get();
            _fillImage.fillAmount = t;
        }
        
        public void AssignStats(Stat stat, Stat statMax)
        {
            _stat = stat;
            _statMax = statMax;
            if(_stat != null && _statMax != null)
                enabled = true;
            else
                enabled = false;
        }

        private void OnEnable()
        {
            if (_stat == null)
                enabled = false;
        }
        
        [ContextMenu("Debug")]
        private void Debug()
        {
            CLog.LogRed($"Enabled: {enabled}. Stat: {_stat.Get()}. Max: {_statMax.Get()}. T: {_stat.Get() / _statMax.Get()}");
        }

        private void Update()
        {
            if (_text != null)
                _text.text = $"{(int)_stat.Get()}";
            var t = _stat.Get() / _statMax.Get();
            _fillImage.fillAmount = t;
            // CLog.Log($"Val {_stat.Val}. Max: {_statMax.Val}");
        }
    }
}