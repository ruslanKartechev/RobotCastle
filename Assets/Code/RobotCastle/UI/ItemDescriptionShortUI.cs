using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ItemDescriptionShortUI : DescriptionUI
    {
        [SerializeField] private bool _dontShowSpell;
        [SerializeField] private UIRectToScreenFitter _rectToScreenFitter;
        [SerializeField] private FadeInOutAnimator _animator;
        [SerializeField] private TextMeshProUGUI _lvlText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _heroIcon;
        [Space(10)]
        [SerializeField] private Image _statsIcon;
        [SerializeField] private TextMeshProUGUI _statsText;
        [Space(10)] 
        [SerializeField] private GameObject _additionalBonusBlock;
        [SerializeField] private TextMeshProUGUI _bonusText;
        

        public override void Show(GameObject source)
        {
            var src = source.GetComponent<IHeroItemDescriptionProvider>();
            ShowCore(src.GetInfo(), src.GetItemIcon());
            var modifiers = source.gameObject.GetComponent<ModifiersContainer>().ModifierIds;
            if(_dontShowSpell)
                _additionalBonusBlock.SetActive(false);
            ShowStats(modifiers, source);
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(src.WorldPosition));
            _animator.FadeIn();
        }

        public void ShowItemOnHero(HeroItemData heroItemData)
        {
            ShowCore(heroItemData.core);
            ShowStats(heroItemData.modifierIds);
        }

        public void ShowCore(DescriptionInfo info, Sprite icon)
        {
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _heroIcon.sprite = icon;
        }
        
        public void ShowCore(CoreItemData itemData)
        {
            var info = DataHelpers.GetItemDescription(itemData);   
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _heroIcon.sprite = DataHelpers.GetItemIcon(itemData);
        }

        public void ShowStats(List<string> modifierIds, GameObject source = null)
        {
            var didFindStat = false;
            var didFindBonus = false;
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            var modDb = ServiceLocator.Get<ModifiersDataBase>();
            foreach (var modId in modifierIds)
            {
                var mod = modDb.GetModifier(modId);
                if (mod is StatsModifierProvider statMod)
                {
                    if (didFindStat)
                        continue;
                    didFindStat = true;
                    _statsIcon.sprite = viewDb.GetStatIcon(statMod.StatType);
                    _statsText.text = $"{Mathf.RoundToInt(statMod.AddedPercent * 100f)}";
                    if (_dontShowSpell)
                        break;
                }
                else if (!didFindBonus)
                {
                    didFindBonus = true;
                    _bonusText.text = mod.GetDescription(source);
                    _additionalBonusBlock.SetActive(true);
                }
                if (didFindBonus && didFindStat)
                    break;
            }
            if(!didFindBonus)
                _additionalBonusBlock.SetActive(false);
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
    }
}