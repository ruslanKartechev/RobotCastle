using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class ItemDescriptionShortUI : DescriptionUI
    {
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
            var src = source.GetComponent<UnitItemDescriptionProvider>();
            var info = src.GetInfo();
            _lvlText.text = info.parts[1];
            _nameText.text = info.parts[0];
            _heroIcon.sprite = src.GetItemIcon();
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(src.WorldPosition));
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            var modifiers = source.gameObject.GetComponent<ModifiersContainer>().Modifiers;
            if (src.Lvl < 2)
                _additionalBonusBlock.SetActive(false);
            else
                _additionalBonusBlock.SetActive(true);
            var didFindStat = false;
            var didFindBonus = false;
            foreach (var mod in modifiers)
            {
                if (mod is StatsModifierProvider statMod)
                {
                    if (didFindStat)
                        continue;
                    didFindStat = true;
                    _statsIcon.sprite = viewDb.GetStatIcon(statMod.StatType);
                    _statsText.text = statMod.GetDescription(source);
                }
                else if (!didFindBonus)
                {
                    didFindBonus = true;
                    _bonusText.text = mod.GetDescription(source);
                }
                if (didFindBonus && didFindStat)
                    break;
            }
            if (!didFindStat)
            {
                CLog.LogRed($"Did not find any StatsModifierProvider on {source.gameObject.name}");
            }
            _animator.FadeIn();
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
    }
}