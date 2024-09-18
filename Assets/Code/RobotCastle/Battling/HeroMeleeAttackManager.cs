using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroMeleeAttackManager : MonoBehaviour, IHeroAttackManager
    {
        private static readonly int AttackId = Animator.StringToHash("Attack");
        private IDamageReceiver _target;
        
        public HeroController Hero { get; set; }
        public IDamageReceiver CurrentTarget => _target;
     
        
        public void BeginAttack(Transform targetTransform, IDamageReceiver target)
        {
            _target = target;
            Hero.HeroView.AnimationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.HeroView.AnimationEventReceiver.OnAttackEvent += OnAttack;
            Hero.HeroView.animator.SetBool(AttackId, true);
            
        }

        public void Stop()
        {
            Hero.HeroView.AnimationEventReceiver.OnAttackEvent -= OnAttack;
            Hero.HeroView.animator.SetBool(AttackId, false);
        }

        private void OnAttack()
        {
            var amount = Hero.HeroView.Stats.Attack.Val;
            var damageArgs = new DamageArgs(amount, EDamageType.Physical);
            // CLog.Log($"Damage from: {gameObject.name}. {damageArgs.GetStr()}");
            _target.TakeDamage(damageArgs);
        }
    }
}