﻿using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using Newtonsoft.Json;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;
using UnityEditor.Animations;

namespace RobotCastle.Testing
{
    public class AttackSpeedTester : MonoBehaviour
    {
        [SerializeField] private bool _logAttackDelays;
        [Space(10)]
        [SerializeField] private float _testSpeed;
        [SerializeField] private List<AnimatorController> _animationControllers;
        [SerializeField] private SimplePoolsManager _poolsManager;
        [SerializeField] private TestHeroesFactory _heroesFactory;

        private void Awake()
        {
            ServiceLocator.Bind<ISimplePoolsManager>(_poolsManager);
            ServiceLocator.Bind<SimplePoolsManager>(_poolsManager);
        }

        [ContextMenu("Set Animation Speed")]
        public void SetAnimationSpeed()
        {
            foreach (var ac in _animationControllers)
            {
                SetSpeed(ac, "Attack", _testSpeed);
            }
        }
        
        [ContextMenu("Set Animation Speed From Save")]
        public void SetAnimationSpeedFromSave()
        {
            var dataSave = ReadAnimationData();
            if (dataSave == null)
            {
                CLog.Log("data is null!");
                return;
            }
            var dataList = dataSave.speedData;
            foreach (var ac in _animationControllers)
            {
                var sd = dataList.Find((t) => t.animatorName.Contains(ac.name) || ac.name.Contains(t.animatorName));
                if (sd == null)
                {
                    CLog.LogYellow($"No recorded data found on {ac.name}");
                    continue;
                }
                var speed = sd.speed;
                if (sd.speed == 0)
                {
                    CLog.LogRed($"{ac.name} recorded speed is 0");
                    speed = 1f;
                }
                SetSpeed(ac, "Attack", speed);
                CLog.Log($"Set speed for: {ac.name} Attack animation to: {speed}");
            }
        }

        public void SetSpeed(AnimatorController controller, string animName, float setSpeed)
        {
            foreach (var layer in controller.layers)
            {
                var state = layer.stateMachine.states.Find((clip) => clip.state.name.Contains(animName)).state;
                if (state == default)
                {
                    CLog.Log($"Couldn't find \"{animName}\" on {controller.name}"); 
                    return;
                }
                state.speed = setSpeed;
                UnityEditor.EditorUtility.SetDirty(controller);
            }
        }
  
        public void DeleteAll() => _heroesFactory.DeleteAll();

        public void Spawn()
        {
            _heroesFactory.SpawnAll();
        }

        public void StartAttack()
        {
            var target = new FakeTarget();
            var heroes = _heroesFactory.AllSpawned;
            foreach (var hero in heroes)
            {
                if (hero.View.AttackManager == null)
                {
                    CLog.LogRed("hero.HeroView.AttackManager is null");
                    continue;
                }

                if (hero.TryGetComponent(out AttackTimer timer) == false)
                {
                    timer = hero.gameObject.AddComponent<AttackTimer>();
                }
                timer.enabled = true;
                timer.DoLog = _logAttackDelays;
                switch (hero.View.AttackManager)
                {
                    case HeroRangedAttackManager:
                        var aimTarget = new GameObject($"attackTarget_{hero.gameObject.name}").transform;
                        aimTarget.position = hero.transform.TransformPoint(new Vector3(0, 1.25f, 2f));
                        hero.View.AttackManager.BeginAttack(aimTarget, target);
                        break;
                    case HeroMeleeAttackManager:
                        hero.View.AttackManager.BeginAttack(transform, target);
                        break;
                    default:
                        CLog.Log("unknown attack manager type ");
                        hero.View.AttackManager.BeginAttack(transform, target);
                        break;
                }
            }
        }

        public void RecordAllAverageSpeed()
        {
            var dataList = new List<SpeedPerAnimator>(50);
            foreach (var spawned in _heroesFactory.AllSpawned)
            {
                if(spawned == null)
                    continue;
                var timer = spawned.gameObject.GetComponent<AttackTimer>();
                if (timer == null)
                {
                    CLog.Log("Timer not assigned");
                    continue;
                }
                var perAnim = new SpeedPerAnimator(timer.AnimatorName, timer.DelayBetween);
                dataList.Add(perAnim);
            }    
            CLog.LogGreen($"Recorded: {dataList.Count} animation speed data");
            var data = new SpeedPerAnimatorData(dataList);
            var dataString = JsonConvert.SerializeObject(data);
            File.WriteAllText(PathToSave(), dataString);
        }

        public SpeedPerAnimatorData ReadAnimationData()
        {
            var text = Resources.Load<TextAsset>(PathToLoad());
            if (text == null)
            {
                CLog.Log(PathToLoad() + " Was not loaded!");
                return null;
            }
            var data = JsonConvert.DeserializeObject<SpeedPerAnimatorData>(text.text);
            return data;
        }

        private string PathToLoad() => "AnimationSpeedData";
        
        private string PathToSave()
        {
            var path = Application.streamingAssetsPath;
            path = path.Replace("StreamingAssets", "Resources");
            path = Path.Join(path,"AnimationSpeedData.json");
            return path;
        }
        
        private class FakeTarget : IDamageReceiver
        {
            public bool IsDamageable => true;

            public void TakeDamage(DamageArgs args)
            {
                
            }
        }
    }
    
    
    [System.Serializable]
    public class SpeedPerAnimatorData
    {
        public List<SpeedPerAnimator> speedData;
        
        public SpeedPerAnimatorData(){}

        public SpeedPerAnimatorData(List<SpeedPerAnimator> data) => speedData = data;
    }
    
    
        
    [System.Serializable]
    public class SpeedPerAnimator
    {
        public string animatorName;
        public float speed;
        
        public SpeedPerAnimator(){}

        public SpeedPerAnimator(string animatorName, float speed)
        {
            this.animatorName = animatorName;
            this.speed = speed;
        }
    }
}