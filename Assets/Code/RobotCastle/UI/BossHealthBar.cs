using UnityEngine;

namespace RobotCastle.UI
{
    public class BossHealthBar : BarTrackerUI
    {
        public BarTrackerUI health => _health;
        public BarTrackerUI mana => _mana;
        
        [SerializeField] private FadeInOutAnimator _animator;
        [SerializeField] private BarTrackerUI _health;
        [SerializeField] private BarTrackerUI _mana;


        public void AnimateIn()
        {
            _animator.On();
            _animator.FadeIn();
        }
    }
}