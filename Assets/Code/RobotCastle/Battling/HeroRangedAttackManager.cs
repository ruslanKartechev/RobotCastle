using System;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public interface IAttackAction
    {
        void Attack(IDamageReceiver target, int animationIndex);
    }
    
    public interface IAttackHitAction
    {
        void Hit(object target);
    }

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

    public class MeleeAttackAction : IAttackAction
    {
        public MeleeAttackAction(HeroComponents components)
        {
            _components = components;
        }
        
        public void Attack(IDamageReceiver target, int animationIndex)
        {
            if (_components.attackSounds.Count > 0)
            {
                var s = _components.attackSounds.Random();
                s.Play();
            }
        }

        private HeroComponents _components;
    }
    
    public class MeleeHitAction : IAttackHitAction
    {
        public MeleeHitAction(HeroComponents components)
        {
            _components = components;
        }
        
        public void Hit(object target)
        {
            var dm = (IDamageReceiver)target;
            if (dm != null)
            {
                _components.damageSource.DamagePhys(dm);
                _components.stats.ManaAdder.AddMana(HeroesConstants.AddedMana);
            }
        }
        
        private HeroComponents _components;
    }
    
    public class RangedHitAction : IAttackHitAction
    {
        public RangedHitAction(HeroComponents components)
        {
            _components = components;
        }

        public void Hit(object target)
        {
            var dm = (IDamageReceiver)target;
            if (dm != null)
            {
                _components.damageSource.DamagePhys(dm);
                _components.stats.ManaAdder.AddMana(HeroesConstants.AddedMana);
            }
        }

        private HeroComponents _components;
    }
    
    
    
    public class HeroRangedAttackManager : MonoBehaviour, IHeroAttackManager
    {
        public event Action OnAttackStep;
        

        public IHeroController Hero { get; set; }
        public IDamageReceiver LastTarget => _target;

        public IProjectileFactory ProjectileFactory
        {
            get => _projectileFactory;
            set => _projectileFactory = value;
        }

        public IAttackAction AttackAction
        {
            get => _attackAction;
            set => _attackAction = value;
        }        
        
        public IAttackHitAction HitAction
        {
            get => _hitAction;
            set => _hitAction = value;
        }        

        
        public void BeginAttack(IDamageReceiver target)
        {
            if (HitAction == null)
                HitAction = new RangedHitAction(Hero.Components);
            if (AttackAction == null)
                AttackAction = new RangedFireAttackAction(Hero.Components, this, HitCallback);

            _target = target;
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
        
        public void SetDefaultActions()
        {
            HitAction = new RangedHitAction(Hero.Components);
            AttackAction = new RangedFireAttackAction(Hero.Components, this, HitCallback);
        }

        private bool _isActive;
        private IDamageReceiver _target;
        private IProjectileFactory _projectileFactory;
        private IAttackAction _attackAction;
        private IAttackHitAction _hitAction;
        
        
        private void OnAttack()
        {
            AttackAction.Attack(_target, 0);
            OnAttackStep?.Invoke();
        }

        private void OnAttack2()
        {
            AttackAction.Attack(_target, 1);
            OnAttackStep?.Invoke();
        }

        private void HitCallback(object target)
        {
            HitAction.Hit(target);
        }

        private void Awake()
        {
            _projectileFactory = gameObject.GetComponent<IProjectileFactory>();
            if (_projectileFactory == null)
            {
                CLog.LogError($"[{nameof(HeroRangedAttackManager)}] _projectileFactory == null !");
            }
        }

    }
}