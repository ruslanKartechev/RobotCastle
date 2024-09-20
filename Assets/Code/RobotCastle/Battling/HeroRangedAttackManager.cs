using System;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroRangedAttackManager : MonoBehaviour, IHeroAttackManager
    {
        public event Action OnAttackStep;
        
        private IProjectileFactory _projectileFactory;
        private Transform _targetTransform;
        private IDamageReceiver _target;

        public HeroController Hero { get; set; }

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
            Hero.HeroView.AnimationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.HeroView.AnimationEventReceiver.OnAttackEvent += OnAttack;
            Hero.HeroView.animator.SetBool(HeroesConfig.Anim_Attack, true);
        }

        public void Stop()
        {
            Hero.HeroView.animator.SetBool(HeroesConfig.Anim_Attack, false);
            Hero.HeroView.AnimationEventReceiver.OnAttackEvent -= OnAttack;
        }

        private void OnAttack()
        {
            var projectile = _projectileFactory.GetProjectile();
            projectile.LaunchProjectile( Hero.HeroView.projectileSpawnPoint, _targetTransform, Hero.HeroView.Stats.ProjectileSpeed, HitCallback, _target);
            // CLog.LogGreen($"Attack event received");
            
            OnAttackStep?.Invoke();
        }

        private void HitCallback(object target)
        {
            var dm = (IDamageReceiver)target;
            if (dm != null)
            {
                var amount = Hero.HeroView.Stats.Attack.Val;
                var damageArgs = new DamageArgs(amount, EDamageType.Physical);
                // CLog.Log($"Damage from: {gameObject.name}. {damageArgs.GetStr()}");
                _target.TakeDamage(damageArgs);
            }
        }
    }
}