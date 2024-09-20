using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroDeathProcessor : MonoBehaviour, IKillProcessor
    {
        [SerializeField] private HeroView _heroView;
        
        public void OnKilled()
        {
            _heroView.HealthManager.SetDamageable(false);
            _heroView.agent.Stop();
            var hero = gameObject.GetComponent<HeroController>();
            hero.MarkDead();
            _heroView.animator.Play("Dead", 0, 0f);
            _heroView.heroUI.AnimateHide();
            _heroView.rb.isKinematic = true;
            _heroView.collider.enabled = false;
        }
    }
}