using RobotCastle.Battling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ItemDescriptionLongUI : DescriptionUI
    {
        [SerializeField] private UIRectToScreenFitter _rectToScreenFitter;
        [SerializeField] private FadeInOutAnimator _animator;
        [SerializeField] private TextMeshProUGUI _lvlText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _heroIcon;

        public override void Show(GameObject source)
        {
            var src = source.GetComponent<UnitItemDescriptionProvider>();
            var info = src.GetInfo();
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _descriptionText.text = info.parts[2];
            _heroIcon.sprite = src.GetItemIcon();
            var modifiers = source.gameObject.GetComponent<ModifiersContainer>().Modifiers;
            _descriptionText.text = "";
            foreach (var mod in modifiers)
            {
                if (mod is not StatsModifierProvider statMod)
                {
                    _descriptionText.text = mod.GetDescription(source);
                    break;
                }
            }
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(src.WorldPosition));
            _animator.FadeIn();
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
    }
    
}