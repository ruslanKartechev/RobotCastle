using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class UnitsUIPanel : MonoBehaviour, IScreenUI
    {
        [SerializeField] private SimplePoolsManager _pool;
        
        private void Awake()
        {
            _pool.Init();
        }

        public void AssignEnemyUI(HeroView view)
        {
            var wrapper = ((UnitUIWrapper)_pool.GetOne("enemy"));
            wrapper.tracker.SetTarget(view.UITrackingPoint);
            view.heroUI = (BattleUnitUI)wrapper.ui;
        }

        public void AssignBossUI(HeroView view)
        {
            var bossUI = ((BossHealthBar)_pool.GetOne("boss"));
            bossUI.tracker.SetTarget(view.UITrackingPoint);
            view.heroUI = bossUI.unitUI;
            bossUI.AnimateIn();
        }
        
        public void AssignHeroUI(HeroView view)
        {
            var wrapper = ((UnitUIWrapper)_pool.GetOne("player"));
            wrapper.tracker.SetTarget(view.UITrackingPoint);
            view.heroUI = (BattleUnitUI)wrapper.ui;
        }

        public void ReturnToPool(UnitUIWrapper uiWrapper)
        {
            _pool.ReturnOne(uiWrapper);
        }
    }
}