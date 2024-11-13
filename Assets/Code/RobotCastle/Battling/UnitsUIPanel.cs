using RobotCastle.Core;
using RobotCastle.UI;
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

        public void AssignEnemyUI(HeroComponents view)
        {
            var wrapper = ((HeroUIWrapper)_pool.GetOne("enemy"));
            wrapper.tracker.SetTarget(view.UITrackingPoint);
            wrapper.transform.position = new Vector3(-1000, -1000, 0);
            view.heroUI = (BattleUnitUI)wrapper.ui;
        }

        public void AssignBossUI(HeroComponents view)
        {
            var bossUI = ((BossHealthBar)_pool.GetOne("boss"));
            bossUI.transform.SetSiblingIndex(transform.childCount-1);
            bossUI.tracker.SetTarget(view.UITrackingPoint);
            view.heroUI = bossUI.unitUI;
            bossUI.AnimateIn();
        }
        
        public void AssignHeroUI(HeroComponents view)
        {
            var wrapper = ((HeroUIWrapper)_pool.GetOne("player"));
            wrapper.tracker.SetTarget(view.UITrackingPoint);
            wrapper.transform.position = new Vector3(-1000, -1000, 0);
            view.heroUI = (BattleUnitUI)wrapper.ui;
        }

        public void ReturnToPool(HeroUIWrapper uiWrapper)
        {
            _pool.ReturnOne(uiWrapper);
        }
    }
}