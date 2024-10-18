using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class SpellDescriptionUI : MonoBehaviour
    {
        public void Show(SpellProvider spell, GameObject hero)
        {
            _nameText.text = spell.GetName();
            _tierText.text = $"Tier {(int)spell.GetTier(hero) + 1}";
            _descriptionText.text = spell.GetDescription(hero);
            _spellIcon.sprite = ServiceLocator.Get<ViewDataBase>().GetSpellIcon(spell.GetId());
        }
        
        public void Show(SpellProvider spell)
        {
            _nameText.text = spell.GetName();
            _tierText.text = $"Tier {(int)spell.GetTier(null) + 1}";
            _descriptionText.text = spell.GetDescription(null);
            if(_spellIcon != null)
                _spellIcon.sprite = ServiceLocator.Get<ViewDataBase>().GetSpellIcon(spell.GetId());
        }

        public void ShowNameAndDescription(SpellProvider spell)
        {
            _nameText.text = spell.GetName();
            _descriptionText.text = spell.GetDescription(null);
        }
        
        public void ShowNameAndDescription(SpellProvider spell, GameObject hero)
        {
            _nameText.text = spell.GetName();
            _descriptionText.text = spell.GetDescription(hero);
        }
        
        public void SetEmpty()
        {
            _descriptionText.text = _nameText.text = _tierText.text = "";
            if(_spellIcon != null)
                _spellIcon.sprite = null;
        }
        
        
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _tierText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _spellIcon;

    }
}