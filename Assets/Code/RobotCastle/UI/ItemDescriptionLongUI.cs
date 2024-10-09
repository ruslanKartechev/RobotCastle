using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SleepDev;

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
                    ShowCore(info, icon);
                    break;
                case EItemDescriptionMode.Modifier:
                    var modifiers = source.gameObject.GetComponent<ModifiersContainer>().ModifierIds;
                    ShowCoreAndModifiers(info, icon, modifiers, src.GetGameObject());
                    break;
            }
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(src.WorldPosition));
            _animator.FadeIn();
        }

        public void ShowItem(HeroItemData itemData)
        {
            var db = ServiceLocator.Get<ViewDataBase>();
            var icon = db.GetUnitItemSpriteAtLevel(itemData.id, itemData.level);
            var descr = ServiceLocator.Get<DescriptionsDataBase>().GetDescriptionByLevel(itemData.core);
            ShowCoreAndModifiers(descr, icon, itemData.modifierIds, null);
        }

        public void ShowBonus(CoreItemData itemData)
        {
            var db = ServiceLocator.Get<ViewDataBase>();
            var sprite = db.GetItemSpriteByTypeAndLevel(itemData);
            var info = ServiceLocator.Get<DescriptionsDataBase>().GetDescriptionByTypeAndLevel(itemData);
            ShowCore(info, sprite);
        }
        
        public void ShowCore(DescriptionInfo info, Sprite icon)
        {
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _descriptionText.text = info.parts[2];
            _heroIcon.sprite = icon;
        }
        
        public void ShowCoreAndModifiers(DescriptionInfo info, Sprite icon, List<string> modifiers, GameObject target)
        {
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _heroIcon.sprite = icon;
            _descriptionText.text = "";
            var modDb = ServiceLocator.Get<ModifiersDataBase>();
            // foreach (var mm in modifiers)
            //     CLog.LogGreen($"Modifier !!!! {mm}");

            var statsMod = (StatsModifierProvider)null;
            var foundNonStat = false;
            foreach (var modId in modifiers)
            {
                var mod = modDb.GetModifier(modId);
                if (mod is StatsModifierProvider ms) //
                {
                    statsMod = ms;
                }                
                if (mod is not StatsModifierProvider) // maybe add mode exceptions here ..
                {
                    _descriptionText.text = mod.GetDescription(target);
                    foundNonStat = true;
                    break;
                }
            }
            if(!foundNonStat && statsMod != null)
                _descriptionText.text = statsMod.GetDescription(target);
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
    }
    
}