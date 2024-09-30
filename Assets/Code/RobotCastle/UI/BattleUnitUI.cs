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
        
        public StarsLevelView Level => _levelView;
        public BarTrackerUI HealthUI => _healthUI;
        public BarTrackerUI ManaUI => _manaUI;
        public ShieldStatBar ShieldBar => _shieldStatBar;

        public void SetMergeMode()
        {
            _unitUILayout.SetMerge();
        }

        public void SetBattleMode()
        {
            _unitUILayout.SetBattle();
        }

        public void AnimateHide()
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
            view.heroUI.HealthUI.AssignStats(view.stats.HealthCurrent, view.stats.HealthMax);
            if(view.stats.ManaMax.Get() > 0)
                view.heroUI.ManaUI.AssignStats(view.stats.ManaCurrent, view.stats.ManaMax);
            else
                view.heroUI.ManaUI.SetVal(0f);
        }

        public void UpdateStatsView(HeroView view)
        {
            view.heroUI.HealthUI.DisplayStats(view.stats.HealthCurrent, view.stats.HealthMax);
            if(view.stats.ManaMax.Get() > 0)
                view.heroUI.ManaUI.DisplayStats(view.stats.ManaCurrent, view.stats.ManaMax);
            else
                view.heroUI.ManaUI.SetVal(0f);
        }
        
    }
}