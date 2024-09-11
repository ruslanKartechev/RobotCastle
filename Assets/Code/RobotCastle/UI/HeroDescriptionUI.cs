using System.Globalization;
using RobotCastle.Battling;
using RobotCastle.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class HeroDescriptionUI : DescriptionUI
    {
        [SerializeField] private UIRectToScreenFitter _rectToScreenFitter;
        [SerializeField] private FadeInOutAnimator _animator;
        [SerializeField] private TextMeshProUGUI _lvlText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _attackText;
        [SerializeField] private TextMeshProUGUI _spellPowerText;
        [SerializeField] private TextMeshProUGUI _attackSpeedText;
        [SerializeField] private BarTrackerUI _health;
        [SerializeField] private BarTrackerUI _mana;
        [SerializeField] private SpellDescriptionUI _spellDescription;
        [SerializeField] private Image _heroIcon;
        private HeroView _heroView;
            
        
        public override void Show(GameObject source)
        {
            var provider = source.GetComponent<HeroDescriptionProvider>();
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(provider.WorldPosition));
            
            _heroView = provider.HeroView;
            var stats = _heroView.Stats;
            _lvlText.text = (stats.HeroLvl + 1).ToString();
            
            var info = ServiceLocator.Get<HeroesDatabase>().GetHeroInfo(stats.HeroId);
            _nameText.text = info.viewInfo.name;
            _heroIcon.sprite = HeroesDatabase.GetHeroSprite(info.viewInfo.iconId);
            
            _attackText.text = (stats.Attack.Val).ToString(CultureInfo.InvariantCulture);
            _spellPowerText.text = (stats.SpellPower.Val).ToString(CultureInfo.InvariantCulture);
            _attackSpeedText.text = (stats.AttackSpeed.Val).ToString(CultureInfo.InvariantCulture);
            _health.Set(stats.HealthCurrent, stats.HealthMax);
            _mana.Set(stats.ManaCurrent, stats.ManaMax);
            _animator.FadeIn();
        }

        public override void Hide()
        {
            _animator.FadeOut();
        }
    }
}