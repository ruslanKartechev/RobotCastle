using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ChapterRewardItemUI : MonoBehaviour
    {
        public void SetIconAndCount(Sprite icon, int count)
        {
            _icon.sprite = icon;
            _text.text = $"x{count}";
        }

        public void On() => gameObject.SetActive(true);

        public void Off() => gameObject.SetActive(false);


        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _icon;
    }
}