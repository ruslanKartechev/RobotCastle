using RobotCastle.Core;
using RobotCastle.Merging;
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
            var hero = gameObject.GetComponent<IHeroController>();
            hero.MarkDead();
            hero.Battle.AttackPositionCalculator.RemoveUnit(_heroView.movement);
            _heroView.movement.SetNullTargetCell();
            _heroView.rb.isKinematic = true;
            _heroView.collider.enabled = false;
            gameObject.SetActive(false);
            if (ServiceLocator.GetIfContains<ISimplePoolsManager>(out var pool))
            {
                var particles = pool.GetOne("death_particles") as DeathParticles;
                particles.Show(transform.position);
            }
            
        }

    }
}