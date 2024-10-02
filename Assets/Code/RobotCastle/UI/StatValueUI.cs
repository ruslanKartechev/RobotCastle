using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class StatValueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void OnNewValueSet(int newVal, int prevVal)
        {
            _text.text = newVal.ToString();
        }

        public void Set(int newVal)
        {
            _text.text = newVal.ToString();
        }

    }
}