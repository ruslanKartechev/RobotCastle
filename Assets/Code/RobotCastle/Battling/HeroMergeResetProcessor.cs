using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroMergeResetProcessor : MonoBehaviour, IHeroMergeResetProcessor
    {
        [SerializeField] private HeroView _heroView;

        public void ResetForMerge()
        {
            _heroView.Stats.HealthReset.Reset(_heroView);
            _heroView.Stats.ManaReset.Reset(_heroView);
            _heroView.heroUI.UpdateStatsView(_heroView);
            
            _heroView.HealthManager.SetDamageable(true);
            _heroView.animator.WriteDefaultValues();
            _heroView.heroUI.Show();
            _heroView.AttackInfo.Reset();
            gameObject.SetActive(true);
        }
    }
}