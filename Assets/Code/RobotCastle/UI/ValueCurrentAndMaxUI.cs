using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ValueCurrentAndMaxUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _fillImage;
        private int _max;

        public void SetMax(int max)
        {
            _max = max;
        }

        public void SetValue(int current, int prev)
        {
            _text.text = $"{current}/{_max}";
            _fillImage.fillAmount = (float)current / _max;
        }
        
    }
}