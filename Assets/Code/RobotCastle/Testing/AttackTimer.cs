using System.Collections;
using System.Collections.Generic;
using Castle.Core.Internal;
using RobotCastle.Battling;
using SleepDev;
using UnityEditor.Animations;
using UnityEngine;

namespace RobotCastle.Testing
{
    [System.Serializable]
    public class AnimaTimerData
    {
        public string stateName;
        public string motionName;
        public float time;
        
        public Motion motionClip { get; set; }
        public AnimatorState animState { get; set; }
    }
    
    public class AttackTimer : MonoBehaviour
    {
        public float DelayBetween => _lastTimeBetween;
        public string AnimatorName => _animator.runtimeAnimatorController.name;

        public bool DoLog { get; set; } = true;
        public bool IsWaiting { get; set; }
        
        private bool _started;
        private float _lastTime;
        private float _lastTimeBetween;
        private int _hitsCount;
        private const int _minHitsToRecord = 3;
        
        private Animator _animator;
        private bool _didInit;

        
        private void Awake()
        {
            if (_didInit)
                return;
            _didInit = true;
            _animator = gameObject.GetComponent<HeroComponents>().animator;
            var attack = gameObject.GetComponentInChildren<HeroAnimationEventReceiver>();
            if (attack == null)
            {
                CLog.LogRed($"HeroAnimationEventReceiver = null on {gameObject.name}");
                return;
            }
            attack.OnAttackEvent -= OnAttack;
            attack.OnAttackEvent += OnAttack;
        }
        
#if UNITY_EDITOR
        public void Begin(AnimatorController animatorController)
        {
            Awake();
            StartCoroutine(MeasuringAllStates(animatorController));
        }

        private IEnumerator MeasuringAllStates(AnimatorController controller)
        {
            IsWaiting = true;
            const string tempName = "temp_state";
            var tempState = (AnimatorState)null;
            var possibleNames = new List<string>() { "Attack", "Attack_2", "Attack_3" };
            var motions = new List<AnimaTimerData>(3);
            var doesHaveTempState = false;
            foreach (var layer in controller.layers)
            {
                var ss = layer.stateMachine.states.Find((clip) => clip.state.name.Contains(tempName));
                if (ss.state != default)
                {
                    tempState = ss.state;
                    doesHaveTempState = true;
                    break;
                }
            }

            if (!doesHaveTempState)
            {
                tempState = controller.layers[0].stateMachine.AddState(tempName);
            }

            foreach (var animName in possibleNames)
            {
                foreach (var layer in controller.layers)
                {
                    var childState = layer.stateMachine.states.Find((clip) => clip.state.name.Contains(animName));
                    if (childState.state != default)
                    {
                        var tempMo = new AnimaTimerData()
                        {
                            motionName = childState.state.motion.name,
                            stateName = animName,
                            motionClip = childState.state.motion,
                            animState = childState.state
                        };
                        motions.Add(tempMo);
                        break;
                    }
                }

            }
            var msg = $"{gameObject.name}. Found Attack motions ({motions.Count}): ";
            foreach (var mo in motions)
                msg += $"{mo.stateName}, ";
            CLog.LogWhite(msg);
            
            yield return null;
            yield return null;
            yield return null;

            foreach (var data in motions)
            {
                _hitsCount = 0;
                // _animator.StopPlayback();
                yield return null;
                tempState.motion = data.motionClip;
                tempState.speed = 1f;
                yield return null;
                _animator.Play(tempName, 0, 0);
                // _animator.StartPlayback();
                while (_hitsCount < _minHitsToRecord)
                {
                    yield return null;
                }
                data.time = _lastTimeBetween;
                data.animState.speed = _lastTimeBetween;
            }
            yield return null;
            CLog.Log($"[{gameObject.name}] Timer work done!");
            _animator.Play("Idle", 0, 0);
            yield return null;
        }
        
        
        
        #endif
        
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
            _hitsCount++;
        }
    }
}