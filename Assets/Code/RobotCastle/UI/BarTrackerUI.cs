using System.Globalization;
using RobotCastle.Battling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class BarTrackerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _fillImage;
        private Stat _stat;
        private Stat _statMax;

        public void Set(Stat stat, Stat statMax)
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
        
        private void Set()
        {
            if(_text != null)
                _text.text = _stat.Val.ToString(CultureInfo.InvariantCulture);
            var t = _stat.Val / _stat.BaseVal;
            _fillImage.fillAmount = t;
        }

        private void Update()
        {
            Set();
        }
    }
}