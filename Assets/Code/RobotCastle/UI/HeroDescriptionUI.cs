using System.Collections.Generic;
using System.Globalization;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
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
        [SerializeField] private List<ItemDescriptionShortUI> _itemsUI;
        [SerializeField] private float _addedWidth = 533.6f;
        private GameObject _src;
        private MergeUnitRangeHighlighter _rangeHighlighter;
        
        public override void Show(GameObject source)
        {
            _src = source;
            
            var provider = source.GetComponent<HeroDescriptionProvider>();
            
            var stats = provider.HeroView.Stats;
            var info = ServiceLocator.Get<HeroesDatabase>().GetHeroViewInfo(stats.HeroId);
            SpellProvider spell = null;
            var spells = source.GetComponent<IModifiersContainer>();
            if ((SpellProvider)spells.currentSpell != null)
                spell = (SpellProvider)spells.currentSpell;
            else
                spell = (SpellProvider)spells.defaultSpell;
            Show(stats, info, spell);

            var items = source.GetComponent<IUnitsItemsContainer>();
            var hasItems = false;
            if (items != null)
            {
                var count = items.ItemsCount;
                for(var i = count; i < _itemsUI.Count; i++)
                    _itemsUI[i].gameObject.SetActive(false);
                for (var i = 0; i < count; i++)
                {
                    _itemsUI[i].gameObject.SetActive(true);
                    var itemData = items.Items[i];
                    _itemsUI[i].ShowCore(itemData);
                    hasItems = true;
                }
            }

            var addedWidth = hasItems ? _addedWidth : 0;
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(provider.WorldPosition), addedWidth);

            _animator.FadeIn();
            ShowRange(_src);
        }

        public void ShowRange(GameObject heroGo)
        {
            var range = new MergeUnitRangeHighlighter(heroGo, ServiceLocator.Get<IGridView>());
            var unit = heroGo.GetComponent<IItemView>();
            var center = new Vector2Int(unit.itemData.pivotX, unit.itemData.pivotY);
            range.UpdateForUnderCell(center);
            range.ShowUnderCell(center);
            _rangeHighlighter = range;
        }
        
        public void Show(HeroStatsContainer stats, HeroViewInfo viewInfo, SpellProvider spellProvider)
        {
            _attackText.text = (stats.Attack.Val).ToString(CultureInfo.InvariantCulture);
            _spellPowerText.text = (stats.SpellPower.Val).ToString(CultureInfo.InvariantCulture);
            _attackSpeedText.text = (stats.AttackSpeed.Val).ToString(CultureInfo.InvariantCulture);
            _health.Set(stats.HealthCurrent, stats.HealthMax);
            _mana.Set(stats.ManaCurrent, stats.ManaMax);
            _lvlText.text = (stats.MergeTier + 1).ToString();

            _nameText.text = viewInfo.name;
            _heroIcon.sprite = HeroesDatabase.GetHeroSprite(viewInfo.iconId);
            
            _spellDescription.Show(spellProvider, _src);
        }
        
        public override void Hide()
        {
            _animator.FadeOut();
            _rangeHighlighter?.Clear();
            _rangeHighlighter = null;
        }
        
    }
}