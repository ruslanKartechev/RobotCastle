using System;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
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

        private void Start()
        {
            if (Hero != null)
            {
                HitAction = new RangedHitAction(Hero.Components);
                AttackAction = new RangedFireAttackAction(Hero.Components, this, HitCallback);
            }
            _projectileFactory = gameObject.GetComponent<IProjectileFactory>();
            if (_projectileFactory == null)
            {
                CLog.LogError($"[{nameof(HeroRangedAttackManager)}] _projectileFactory == null !");
            }
        }

    }
}