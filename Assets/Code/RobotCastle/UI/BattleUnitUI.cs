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
        [SerializeField] private CanvasGroup _canvasGroup;
        
        public StarsLevelView Level => _levelView;
        public BarTrackerUI HealthUI => _healthUI;
        public BarTrackerUI ManaUI => _manaUI;

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
            view.heroUI.HealthUI.AssignStats(view.Stats.HealthCurrent, view.Stats.HealthMax);
            view.heroUI.ManaUI.AssignStats(view.Stats.ManaCurrent, view.Stats.ManaMax);
        }

        public void UpdateStatsView(HeroView view)
        {
            view.Stats.ManaCurrent.SetBaseAndCurrent(0);
            view.heroUI.HealthUI.DisplayStats(view.Stats.HealthCurrent, view.Stats.HealthMax);
            view.heroUI.ManaUI.DisplayStats(view.Stats.ManaCurrent, view.Stats.ManaMax);
        }
        
    }
    
}