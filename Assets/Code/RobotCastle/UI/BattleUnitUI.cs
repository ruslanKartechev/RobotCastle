using RobotCastle.Battling;
using SleepDev;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BattleUnitUI : MonoBehaviour
    {
        [SerializeField] private UnitUILayout _unitUILayout;
        [SerializeField] private StarsLevelView _levelView;
        [SerializeField] private BarTrackerUI _healthUI;
        [SerializeField] private BarTrackerUI _manaUI;
        [SerializeField] private ShieldStatBar _shieldStatBar;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _damagePoint;
        [SerializeField] private UnitItemsContainerView _unitItemsContainer;

        public UnitItemsContainerView Items => _unitItemsContainer;
        public RectTransform DamagePoint => _damagePoint;
        public StarsLevelView Level => _levelView;
        public BarTrackerUI HealthUI => _healthUI;
        public BarTrackerUI ManaUI => _manaUI;
        public ShieldStatBar ShieldBar => _shieldStatBar;

        public void ReplaceHealthAndManaBar(BarTrackerUI health, BarTrackerUI mana)
        {
            if(_healthUI != null && _healthUI != health)
                _healthUI.gameObject.SetActive(false);
            if(_manaUI != null && _manaUI != mana)
                _manaUI.gameObject.SetActive(false);
            _healthUI = health;
            _manaUI = mana;
        }

        public void SetMergeMode()
        {
            _unitUILayout.SetMerge();
        }

        public void SetBattleMode()
        {
            _unitUILayout.SetBattle();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            // _canvasGroup.DOFade(0f, .2f);   
        }

        public void Show()
        {
            _canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
        }
        
        public void AssignStatsTracking(HeroView view)
        {
            _healthUI.AssignStats(view.stats.HealthCurrent, view.stats.HealthMax);
            if(view.stats.ManaMax.Get() > 0)
                _manaUI.AssignStats(view.stats.ManaCurrent, view.stats.ManaMax);
            else
                _manaUI.SetVal(0f);
        }

        public void UpdateStatsView(HeroView view)
        {
            _healthUI.DisplayStats(view.stats.HealthCurrent, view.stats.HealthMax);
            if(view.stats.ManaMax.Get() > 0)
                _manaUI.DisplayStats(view.stats.ManaCurrent, view.stats.ManaMax);
            else
                _manaUI.SetVal(0f);
        }
        
    }
}