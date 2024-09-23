using RobotCastle.Battling;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class AttackTimer : MonoBehaviour
    {
        private bool _started;
        private float _lastTime;
        private float _lastTimeBetween;
        
        private IHeroAttackManager _attack;
        private Animator _animator;

        public float DelayBetween => _lastTimeBetween;
        
        public string AnimatorName => _animator.runtimeAnimatorController.name;

        public bool DoLog { get; set; } = true;
        
        private void Awake()
        {
            _animator = gameObject.GetComponent<HeroView>().animator;
            
            var attack = gameObject.GetComponent<IHeroAttackManager>();
            if (attack == null)
            {
                CLog.LogRed($"IHeroAttackManager = null on {gameObject.name}");
                return;
            }
            attack.OnAttackStep += OnAttack;
            _attack = attack;
        }

        private void OnAttack()
        {
            if (!_started)
            {
                _started = true;
                _lastTime = Time.time;
                return;
            }
            var diff = Time.time - _lastTime;
            if(DoLog)
                CLog.Log($"[{gameObject.name}] Time Diff: {diff:N4}");
            _lastTime = Time.time;
            _lastTimeBetween = diff;
        }
    }
}