using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroRestartProcessor : MonoBehaviour, IHeroRestartProcessor
    {
        [SerializeField] private HeroView _heroView;

        public void Restart()
        {
            _heroView.HealthManager.SetDamageable(true);
            _heroView.animator.WriteDefaultValues();
            _heroView.animator.Play("Idle", 0, 0);
            _heroView.heroUI.Show();
        }
    }
}