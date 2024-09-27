using System.Collections.Generic;
using System.Globalization;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
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
        [SerializeField] private HeroDescriptionLayout _descriptionLayout;
        [SerializeField] private Image _heroIcon;
        [SerializeField] private List<ItemDescriptionShortUI> _itemsUI;
        [SerializeField] private float _addedWidth = 533.6f;
        private GameObject _src;
        private MergeUnitRangeHighlighter _rangeHighlighter;
        
        public override void Show(GameObject source)
        {
            _src = source;
            var provider = source.GetComponent<HeroDescriptionProvider>();
            var stats = provider.HeroView.stats;
            var info = ServiceLocator.Get<HeroesDatabase>().GetHeroViewInfo(stats.HeroId);
            var spells = source.GetComponent<HeroView>().spellsContainer;
            var spell = (SpellProvider)spells.GetCurrentSpell();
            if (spell == null)
            {
                CLog.LogError($"[{source}] does not have a spell!");
            }
            Show(stats, info, spell);

            var heroItems = source.GetComponent<IHeroItemsContainer>();
            var hasItems = false;
            if (heroItems != null)
            {
                var count = heroItems.ItemsCount;
                for(var i = count; i < _itemsUI.Count; i++)
                    _itemsUI[i].gameObject.SetActive(false);
                for (var i = 0; i < count; i++)
                {
                    _itemsUI[i].gameObject.SetActive(true);
                    _itemsUI[i].ShowItemOnHero(heroItems.Items[i]);
                    hasItems = true;
                }
            }

            var addedWidth = hasItems ? _addedWidth : 0;
            _rectToScreenFitter.SetScreenPos(Camera.main.WorldToScreenPoint(provider.WorldPosition), addedWidth);

            _animator.FadeIn();
            var battle = ServiceLocator.Get<Battle>();
            if(battle is { State: BattleState.NotStarted })
                ShowRange(_src);
        }

        public void ShowRange(GameObject heroGo)
        {
            var cell = MergeFunctions.RaycastUnderItem(heroGo);
            if (cell == null)
            {
                CLog.Log($"[{nameof(HeroDescriptionUI)}] Something is wrong! Cannot find cell under {heroGo.name}");
                return;
            }
            var grid = ServiceLocator.Get<GridViewsContainer>().GetGridView(cell.GridId);
            var highlighter = new MergeUnitRangeHighlighter(heroGo, grid);
            var unit = heroGo.GetComponent<IItemView>();
            var center = new Vector2Int(unit.itemData.pivotX, unit.itemData.pivotY);
            highlighter.UpdateForUnderCell(center);
            highlighter.ShowUnderCell(center);
            _rangeHighlighter = highlighter;
        }
        
        
        
        public void Show(HeroStatsManager stats, HeroViewInfo viewInfo, SpellProvider spellProvider)
        {
            const string Color = "#FFFF11";
            var atkTxt = (stats.Attack.Val).ToString(CultureInfo.InvariantCulture);
            var atkSpTxt = (stats.AttackSpeed.Val).ToString(CultureInfo.InvariantCulture);
            string spTxt;
            if (stats.SpellPowerGetter != null)
                spTxt = (stats.SpellPowerGetter.BaseSpellPower).ToString(CultureInfo.InvariantCulture);
            else
                spTxt = "0";
            
            var items = stats.gameObject.GetComponent<HeroItemsContainer>();
            if (items.Items.Count > 0)
            {
                var db = ServiceLocator.Get<ModifiersDataBase>();
                var addedAtk = 0f;
                var addedSp = 0f;
                var addedAtkSpeed = 0f;
                foreach (var itemData in items.Items)
                {
                    foreach (var id in itemData.modifierIds)
                    {
                        var dd = db.GetModifier(id);
                        if (dd is StatsModifierProvider statMod)
                        {
                            switch (statMod.StatType)
                            {
                                case EStatType.Attack:
                                    addedAtk += statMod.AddedPercent;
                                    break;
                                case EStatType.SpellPower:
                                    addedSp += statMod.AddedPercent;
                                    break;
                                case EStatType.AttackSpeed:
                                    addedAtkSpeed += statMod.AddedPercent;
                                    break;
                            }
                        }
                    }
                }
                if (addedAtk > 0)
                {
                    var addedVal = Mathf.RoundToInt(addedAtk * stats.Attack.Val);
                    atkTxt += $"+<color={Color}>{addedVal}</color>";
                    // atkTxt += $"+{addedVal}";
                }
                if (addedSp > 0 && stats.SpellPowerGetter != null)
                {
                    var addedVal = Mathf.RoundToInt(addedSp * stats.SpellPowerGetter.BaseSpellPower);
                    spTxt += $"+<color={Color}>{addedVal}</color>";
                }
                if (addedAtkSpeed > 0)
                {
                    var addedVal = Mathf.RoundToInt(addedAtkSpeed * stats.AttackSpeed.Val);
                    atkSpTxt += $"+<color={Color}>{addedVal}</color>";
                }
            }
            
            _attackText.text = atkTxt;
            _spellPowerText.text = spTxt;
            _attackSpeedText.text = atkSpTxt;
            _health.AssignStats(stats.HealthCurrent, stats.HealthMax);
            _mana.AssignStats(stats.ManaCurrent, stats.ManaMax);
            _lvlText.text = (stats.MergeTier + 1).ToString();

            _nameText.text = viewInfo.name;
            _heroIcon.sprite = HeroesDatabase.GetHeroSprite(viewInfo.iconId);
            if (spellProvider != null)
            {
                _descriptionLayout.SetLong();
                _spellDescription.Show(spellProvider, _src);
            }
            else
            {
                CLog.Log($"[{nameof(HeroDescriptionUI)}] hero spellProvider is null");
                _descriptionLayout.SetShort();
                _spellDescription.SetEmpty();
            }
        }
        
        public override void Hide()
        {
            _animator.FadeOut();
            _rangeHighlighter?.Clear();
            _rangeHighlighter = null;
        }
    }

}