using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class PlayerExperienceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lvlText;
        [SerializeField] private Image _fillImage;

        public void SetPlayerXp(int level, float progress)
        {
            _lvlText.text = (level + 1).ToString();
            _fillImage.fillAmount = progress;
        }
        
    }

}