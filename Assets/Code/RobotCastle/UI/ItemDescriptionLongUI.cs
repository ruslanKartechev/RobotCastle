using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Merging;
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
            var src = source.GetComponent<IHeroItemDescriptionProvider>();
            var info = src.GetInfo();
            var icon = src.GetItemIcon();

            switch (src.Mode)
            {
                case EItemDescriptionMode.DescriptionOnly:
                    Show(info, icon);
                    break;
                case EItemDescriptionMode.Modifier:
                    var modifiers = source.gameObject.GetComponent<ModifiersContainer>().Modifiers;
                    Show(info, icon, modifiers, src.GetGameObject());
                    break;
            }
          
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(src.WorldPosition));
            _animator.FadeIn();
        }

        public void Show(DescriptionInfo info, Sprite icon)
        {
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _descriptionText.text = info.parts[2];
            _heroIcon.sprite = icon;
        }
        
        public void Show(DescriptionInfo info, Sprite icon, List<ModifierProvider> modifiers, GameObject target)
        {
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _heroIcon.sprite = icon;
            _descriptionText.text = "";
            foreach (var mod in modifiers)
            {
                if (mod is not StatsModifierProvider statMod)
                {
                    _descriptionText.text = mod.GetDescription(target);
                    break;
                }
            }
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
    }
    
}