using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.MainMenu;
using RobotCastle.Saving;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class BarracksHeroPanel : MonoBehaviour, IScreenUI
    {
        public MyButton btnGrowth => _btnGrowth;

        public MyButton btnBack => _btnBack;
        
        [SerializeField] private TextMeshProUGUI _statTxtAttack;
        [SerializeField] private TextMeshProUGUI _statTxtSpellPower;
        [SerializeField] private TextMeshProUGUI _statTxtAttackSpeed;
        [SerializeField] private TextMeshProUGUI _statTxtHealth;
        [SerializeField] private TextMeshProUGUI _statTxtRange;
        [SerializeField] private TextMeshProUGUI _statTxtMoveSpeed;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI _txtHeroLevel;
        [SerializeField] private TextMeshProUGUI _txtHeroXp;
        [SerializeField] private TextMeshProUGUI _txtHeroName;
        [SerializeField] private Image _xpFill;
        [SerializeField] private Image _heroIcon;
        [Space(10)]
        [SerializeField] private BarracksHeroSpellView _spellView;
        [Space(10)] 
        [SerializeField] private MyButton _btnGrowth;
        [SerializeField] private MyButton _btnBack;
        private string _heroId;

        public void Show(string id)
        {
            gameObject.SetActive(true);
            _heroId = id;
            UpdateStats();
            _btnGrowth.AddMainCallback(Growth);
            _btnBack.AddMainCallback(Back);
            _btnGrowth.SetInteractable(true);
            _btnBack.SetInteractable(true);
        }

        private void UpdateStats()
        {
            // var viewDb = ServiceLocator.Get<ViewDataBase>();
            var heroesDb = ServiceLocator.Get<HeroesDatabase>();
            var save = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(_heroId);
            var heroInfo = heroesDb.GetHeroInfo(_heroId);
            var stats = heroInfo.stats;
            var lvl = save.level;
            _heroIcon.sprite = ViewDataBase.GetSprite(heroInfo.viewInfo.iconId);
            _txtHeroName.text = heroInfo.viewInfo.name;
            _txtHeroLevel.text = (lvl + 1).ToString();
            _txtHeroXp.text = $"{save.xp}/{save.xpForNext}";
            _xpFill.fillAmount = (float)save.xp / save.xpForNext;
            _spellView.Init(_heroId, heroInfo);

            _statTxtAttack.text = ((int)HeroStatsManager.GetStatByLevel(stats.attack, lvl, 1)).ToString();
            _statTxtSpellPower.text = ((int)HeroStatsManager.GetStatByLevel(stats.spellPower, lvl, 1)).ToString();
            _statTxtAttackSpeed.text = ((int)HeroStatsManager.GetStatByLevel(stats.attackSpeed, lvl, 1)).ToString();
            _statTxtHealth.text = ((int)HeroStatsManager.GetStatByLevel(stats.health, lvl, 1)).ToString();
            _statTxtMoveSpeed.text = ((int)HeroStatsManager.GetMoveSpeed(stats.moveSpeed)).ToString();
            var range = AttackRangeFactory.GetAttackRangeById(_heroId);
            _statTxtRange.text = range.GetStringDescription();
        }

        private void Growth()
        {
            gameObject.SetActive(false);
            var panel = ServiceLocator.Get<IUIManager>().Show<HeroGrowthPanel>(UIConstants.UIHeroGrowth, OnGrowthClosed);
            panel.Show(_heroId);
        }

        private void OnGrowthClosed()
        {
            gameObject.SetActive(true);
            UpdateStats();
        }
        
        private void Back()
        {
            ServiceLocator.Get<TabsSwitcher>().SetBarracksTab();
        }

        public void Close()
        {
            gameObject.SetActive(false);            
        }
    }
}