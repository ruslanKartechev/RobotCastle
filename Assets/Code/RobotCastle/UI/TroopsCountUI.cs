using RobotCastle.Battling;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class TroopsCountUI : MonoBehaviour, ITroopsCountView
    {
        [SerializeField] private TextMeshProUGUI _text;


        public void SetCount(int count, int max)
        {
            _text.text = $"{count}/{max}";
        }

        public void UpdateCount(int count, int max)
        {
            _text.text = $"{count}/{max}";
        }
    }
}