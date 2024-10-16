﻿using RobotCastle.Core;
using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public class HeroDeathProcessor : IKillProcessor
    {
        private HeroComponents _heroView;

        public HeroDeathProcessor(HeroComponents heroView)
        {
            _heroView = heroView;
        }
        
        public void OnKilled()
        {
            _heroView.healthManager.SetDamageable(false);
            _heroView.agent.Stop();
            var hero = _heroView.gameObject.GetComponent<IHeroController>();
            hero.MarkDead();
            hero.Battle.AttackPositionCalculator.RemoveUnit(_heroView.state);
            _heroView.state.SetTargetCellToSelf();
            _heroView.heroUI.Hide();
            _heroView.gameObject.SetActive(false);
            _heroView.processes.StopAll();
            if (ServiceLocator.GetIfContains<ISimplePoolsManager>(out var pool))
            {
                var particles = pool.GetOne("death_particles") as OneTimeParticles;
                particles.Show(_heroView.transform.position);
            }
            
        }

    }
}