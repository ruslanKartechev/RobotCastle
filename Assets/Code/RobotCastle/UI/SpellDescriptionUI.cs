using RobotCastle.Battling;
using TMPro;
using UnityEngine;

namespace RobotCastle.UI
{
    public class SpellDescriptionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _tierText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        public void Show(HeroStats heroStats)
        {
            _nameText.text = "Spell Name";
            _tierText.text = "Tier 1";
            _descriptionText.text = "Description of the spell";
        }
    }
}