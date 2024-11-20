using System;
using SleepDev;

namespace RobotCastle.Battling
{
    public class RangedFireAttackAction : IAttackAction
    {
        public RangedFireAttackAction(HeroComponents components,
            HeroRangedAttackManager rangedAttack,
            Action<object> hitCallback)
        {
            _components = components;
            _rangedAttack = rangedAttack;
            _hitCallback = hitCallback;

        }
            
        public void Attack(IDamageReceiver target, int animationIndex)
        {
            if (target == null || target.GetGameObject() == null) return;
            if(_components.shootParticles != null)
                _components.shootParticles.Play();

            var pp = animationIndex == 0 ? _components.projectileSpawnPoint : _components.projectileSpawnPoint2;
            
            var projectile = _rangedAttack.ProjectileFactory.GetProjectile();
            projectile.LaunchProjectile(pp, target.GetGameObject().transform, 
                _components.stats.ProjectileSpeed, _hitCallback, target);

            if (_components.attackSounds.Count > 0)
            {
                var s = _components.attackSounds.Random();
                s.Play();
            }
        }
        private HeroComponents _components;
        private HeroRangedAttackManager _rangedAttack;
        private Action<object> _hitCallback;
    }
}