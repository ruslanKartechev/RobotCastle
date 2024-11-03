using System.Collections.Generic;
using RobotCastle.Core;

namespace RobotCastle.Battling
{
    public class HeroDeathProcessor : IKillProcessor
    {
        private HeroComponents _components;

        public HeroDeathProcessor(HeroComponents components)
        {
            _components = components;
        }

        public void Kill()
        {
            _components.healthManager.SetDamageable(false);
            _components.movement.Stop();

            var count = _modifiers.Count;
            for (var i = count-1; i >= 0; i--)
            {
                var mod = _modifiers[i];
                mod.OnKilled(_components);
            }
            
            var hero = _components.gameObject.GetComponent<IHeroController>();
            hero.MarkDead();
            hero.Battle.AttackPositionCalculator.RemoveUnit(_components.state);
            _components.state.SetTargetCellToSelf();
            _components.heroUI.Hide();
            _components.gameObject.SetActive(false);
            _components.processes.StopAll();
            
          
            if (ServiceLocator.GetIfContains<ISimplePoolsManager>(out var pool))
            {
                var particles = pool.GetOne("death_particles") as OneTimeParticles;
                particles.Show(_components.transform.position);
            }
        }

        public void AddModifier(IKIllModifier mod)
        {
            if (_modifiers.Contains(mod) == false)
            {
                _modifiers.Add(mod);
            }
        }

        public void RemoveModifier(IKIllModifier mod)
        {
            _modifiers.Remove(mod);
        }

        public void ClearAllModifiers() => _modifiers.Clear();

        private List<IKIllModifier> _modifiers = new (2);

    }
}