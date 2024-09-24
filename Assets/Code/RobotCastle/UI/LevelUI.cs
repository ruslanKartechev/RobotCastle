using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelNumText;
        
        public void SetLevel(int levelIndex, bool animated)
        {
            _levelNumText.text = $"Level {levelIndex + 1}";
        }
        
    }
}