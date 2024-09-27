using System;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroRangedAttackManager : MonoBehaviour, IHeroAttackManager
    {
        public event Action OnAttackStep;

        public IHeroController Hero { get; set; }
        public IProjectileFactory ProjectileFactory
        {
            get => _projectileFactory;
            set => _projectileFactory = value;
        }
        
        private Transform _targetTransform;
        private IDamageReceiver _target;
        
        private IProjectileFactory _projectileFactory;
        

        private void Awake()
        {
            _projectileFactory = gameObject.GetComponent<IProjectileFactory>();
            if (_projectileFactory == null)
            {
                CLog.LogError($"[{nameof(HeroRangedAttackManager)}] _projectileFactory == null !");
            }
        }

        public void BeginAttack(Transform targetTransform, IDamageReceiver target)
        {
            _target = target;
            _targetTransform = targetTransform;
            Hero.View.animationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.View.animationEventReceiver.OnAttackEvent += OnAttack;
            Hero.View.animator.SetBool(HeroesConfig.Anim_Attack, true);
        }

        public void Stop()
        {
            Hero.View.animator.SetBool(HeroesConfig.Anim_Attack, false);
            Hero.View.animationEventReceiver.OnAttackEvent -= OnAttack;
        }

        private void OnAttack()
        {
            var projectile = _projectileFactory.GetProjectile();
            projectile.LaunchProjectile(Hero.View.projectileSpawnPoint, _targetTransform, Hero.View.stats.ProjectileSpeed, HitCallback, _target);
            OnAttackStep?.Invoke();
        }

        private void HitCallback(object target)
        {
            var dm = (IDamageReceiver)target;
            if (dm != null)
            {
                var damage = Hero.View.damageSource.GetDamage();
                Hero.View.stats.ManaAdder.AddMana(damage.physDamage * HeroesConfig.ManaGainDamageMultiplier);
                _target.TakeDamage(damage);
            }
        }
    }
}