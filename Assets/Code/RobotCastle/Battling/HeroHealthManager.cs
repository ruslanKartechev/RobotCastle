using System;
using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroHealthManager : IHeroHealthManager, IDamageReceiver
    {
        public HeroHealthManager(HeroComponents heroComponents)
        {
            _components = heroComponents;
        }
        
        public GameObject GetGameObject() => _components.gameObject;
        
        public DamageReceivedArgs TakeDamage(HeroDamageArgs args)
        {
            if (!_isDamageable)
                return default;
            var stats = _components.stats;
            var DEF = 0f;
            switch (args.type)
            {
                case EDamageType.Magical:
                    DEF = stats.PhysicalResist.Val;
                    break;
                case EDamageType.Physical:
                    DEF = stats.MagicalResist.Val;
                    break;
            }
            args.amount = HeroesManager.ReduceDamageByDef(args.amount, DEF);

            foreach (var mod in _modifiers)
            {
                args = mod.Apply(args);
                if (args.amount <= 0)
                    break;
            }

            foreach (var mod in _deleteQueue)
                _modifiers.Remove(mod);
            _deleteQueue.Clear();
            
            var damageAmount = args.amount;
            var health = stats.HealthCurrent.Get();

            if (damageAmount > 0)
            {
                ServiceLocator.Get<IDamageDisplay>().ShowAtScreenPos((int)damageAmount, args.type, _components.heroUI.DamagePoint.position);
                health -= damageAmount;
                if (health < 0) 
                    health = 0;
            }
            StatsCollector.AddDamageReceived(_components.GUID, args.type, (int)damageAmount);
            stats.HealthCurrent.Val = health;
            if (health <= 0)
            {
                if(_components.killProcessor != null)
                    _components.killProcessor.Kill();
                else
                    CLog.LogRed($"[{nameof(HeroHealthManager)}] IKillProcessor is null {_components.name}");
            }
            else
            {
                if(_components.Flicker != null)
                    _components.Flicker.Flick();
            }
            return new DamageReceivedArgs(damageAmount, health <= 0);
        }

        public void SetDamageable(bool damageable) => _isDamageable = damageable;

        public IBattleDamageStatsCollector StatsCollector { get; set; }

        public void AddModifier(IDamageTakenModifiers mod)
        {
            if (_modifiers.Contains(mod) == false)
            {
                _modifiers.Add(mod);
                _modifiers.Sort((a, b) => a.priority.CompareTo(b.priority));
            }
        }

        public void RemoveModifier(IDamageTakenModifiers mod)
        {
            _deleteQueue.Add(mod);   
        }

        public void ClearAllModifiers()
        {
            // for (var i = _modifiers.Count - 1; i >= 0; i++)
                    // _modifiers.RemoveAt(i);
            _modifiers.Clear();
            _deleteQueue.Clear();
        }
        
        private bool _isDamageable = true;
        private HeroComponents _components;
        private List<IDamageTakenModifiers> _modifiers = new (10);
        private List<IDamageTakenModifiers> _deleteQueue = new (5);

    }
}