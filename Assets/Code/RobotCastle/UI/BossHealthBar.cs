using RobotCastle.Battling;
using UnityEngine;

namespace RobotCastle.UI
{
    public class BossHealthBar : UnitUIWrapper
    {
        public BattleUnitUI unitUI => _ui;
        
        [SerializeField] private FadeInOutAnimator _animator;
        [SerializeField] private BattleUnitUI _ui;

        public void AnimateIn()
        {
            _animator.On();
            _animator.FadeIn();
        }
    }
}