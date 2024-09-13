using RobotCastle.Battling;
using RobotCastle.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class SpellDescriptionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _tierText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _spellIcon;

        public void Show(SpellProvider spell, GameObject hero)
        {
            _nameText.text = spell.GetName();
            _tierText.text = $"Tier {spell.GetTier(hero) + 1}";
            _descriptionText.text = spell.GetDescription(hero);
            _spellIcon.sprite = ServiceLocator.Get<ViewDataBase>().GetSpellIcon(spell.GetId());
        }
        
        public void Show(SpellProvider spell)
        {
            _nameText.text = spell.GetName();
            _tierText.text = $"Tier {spell.GetTier(null) + 1}";
            _descriptionText.text = spell.GetDescription(null);
            _spellIcon.sprite = ServiceLocator.Get<ViewDataBase>().GetSpellIcon(spell.GetId());
        }
        
        
    }
}