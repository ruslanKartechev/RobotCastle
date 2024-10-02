using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Saving;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.UI
{
    public class BarracksHeroDescriptionPanel : MonoBehaviour, IScreenUI
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
        [SerializeField] private Image _spellIcon;
        [Space(10)] 
        [SerializeField] private MyButton _btnGrowth;
        [SerializeField] private MyButton _btnBack;


        public void Show(string id)
        {
            var viewDb = ServiceLocator.Get<ViewDataBase>();
            var descriptionDb = ServiceLocator.Get<DescriptionsDataBase>();
            var heroesDb = ServiceLocator.Get<HeroesDatabase>();
            var save = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().GetSave(id);
            var heroData = heroesDb.GetHeroInfo(id);
            var stats = heroData.stats;
            var lvl = save.level;
            _heroIcon.sprite = ViewDataBase.GetHeroSprite(heroData.viewInfo.iconId);
            _txtHeroName.text = heroData.viewInfo.name;
            _txtHeroLevel.text = (lvl + 1).ToString();
            _txtHeroXp.text = $"{save.xp}/{save.xpForNext}";
            _xpFill.fillAmount = (float)save.xp / save.xpForNext;
            _spellIcon.sprite = viewDb.GetSpellIcon(heroData.spellInfo.mainSpellId);

            _statTxtAttack.text = ((int)HeroStatsManager.GetStatByLevel(stats.attack, lvl, 1)).ToString();
            _statTxtSpellPower.text = ((int)HeroStatsManager.GetStatByLevel(stats.spellPower, lvl, 1)).ToString();
            _statTxtAttackSpeed.text = ((int)HeroStatsManager.GetStatByLevel(stats.attackSpeed, lvl, 1)).ToString();
            _statTxtHealth.text = ((int)HeroStatsManager.GetStatByLevel(stats.health, lvl, 1)).ToString();
            _statTxtMoveSpeed.text = ((int)HeroStatsManager.GetMoveSpeed(stats.moveSpeed)).ToString();
            var range = AttackRangeFactory.GetAttackRangeById(id);
            _statTxtRange.text = range.GetStringDescription();
            
            _btnGrowth.AddMainCallback(Growth);
            _btnBack.AddMainCallback(Back);
        }

        private void Growth()
        {
            CLog.LogBlue($"Growth");
        }

        private void Back()
        {
            CLog.LogBlue($"Back");
        }

        public void Close()
        {
            gameObject.SetActive(false);            
        }
    }
}