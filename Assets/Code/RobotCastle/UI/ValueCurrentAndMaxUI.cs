using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ValueCurrentAndMaxUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _fillImage;

        public void SetValue(int current, int max)
        {
            _text.text = $"{current}/{max}";
            _fillImage.fillAmount = (float)current / max;
        }
        
    }
}