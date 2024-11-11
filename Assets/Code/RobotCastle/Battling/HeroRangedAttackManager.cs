using System;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroRangedAttackManager : MonoBehaviour, IHeroAttackManager
    {
        public event Action OnAttackStep;
        public event Action OnHit;

        public IHeroController Hero { get; set; }
        public IDamageReceiver LastTarget => _target;

        public IProjectileFactory ProjectileFactory
        {
            get => _projectileFactory;
            set => _projectileFactory = value;
        }
        
        public void BeginAttack(IDamageReceiver target)
        {
            _target = target;
            _targetTransform = target.GetGameObject().transform;
            if (!_isActive)
            {
                _isActive = true;
                Hero.Components.state.isAttacking = true;
                Hero.Components.animationEventReceiver.OnAttackEvent -= OnAttack;
                Hero.Components.animationEventReceiver.OnAttackEvent += OnAttack;
                Hero.Components.animationEventReceiver.OnAttackEvent2 -= OnAttack2;
                Hero.Components.animationEventReceiver.OnAttackEvent2 += OnAttack2;
                Hero.Components.animator.SetBool(HeroesConstants.Anim_Attack, true);
            }
        }
        
        public void Stop()
        {
            if (!_isActive) return;
            _isActive = false;
            Hero.Components.state.isAttacking = false;
            Hero.Components.animator.SetBool(HeroesConstants.Anim_Attack, false);
            Hero.Components.animationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.Components.animationEventReceiver.OnAttackEvent2 -= OnAttack2;
        }

        private Transform _targetTransform;
        private IDamageReceiver _target;
        private IProjectileFactory _projectileFactory;
        private bool _isActive;
        
        private void OnAttack()
        {
            Attack(Hero.Components.projectileSpawnPoint);
        }

        private void OnAttack2()
        {
            Attack(Hero.Components.projectileSpawnPoint2);
        }

        private void Attack(Transform projectilePoint)
        {
            if(Hero.Components.shootParticles != null)
                Hero.Components.shootParticles.Play();
            var projectile = _projectileFactory.GetProjectile();
            projectile.LaunchProjectile(projectilePoint, _targetTransform, 
                Hero.Components.stats.ProjectileSpeed, HitCallback, _target);

            if (Hero.Components.attackSounds.Count > 0)
            {
                var s = Hero.Components.attackSounds.Random();
                s.Play();
            }
            OnAttackStep?.Invoke();
        }

        private void Awake()
        {
            _projectileFactory = gameObject.GetComponent<IProjectileFactory>();
            if (_projectileFactory == null)
            {
                CLog.LogError($"[{nameof(HeroRangedAttackManager)}] _projectileFactory == null !");
            }
        }

        private void HitCallback(object target)
        {
            var dm = (IDamageReceiver)target;
            if (dm != null)
            {
                Hero.Components.damageSource.DamagePhys(_target);
                Hero.Components.stats.ManaAdder.AddMana(HeroesConstants.AddedMana);
            }
            OnHit?.Invoke();
        }

    }
}