using System;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroHealthManager : IHeroHealthManager, IHeroDamageReceiver
    {
        
        public bool LogDamage { get; set; }
        
        public event Action OnAfterDamage;
        public event Action OnDamagePreApplied;
        public event Action OnDamageAttempted;
        
        public GameObject GetGameObject() => _view.gameObject;

        public HeroHealthManager(HeroView heroView)
        {
            _view = heroView;
        }
        
        private HeroView _view;
        private bool _isDamageable = true;
        
        public DamageArgs TakeDamage(DamageArgs args)
        {
            OnDamageAttempted?.Invoke();
            if (!_isDamageable)
                return default;
            OnDamagePreApplied?.Invoke();
            var stats = _view.stats;
            var physDam = HeroesManager.ReduceDamageByDef(args.physDamage, stats.PhysicalResist.Val);
            var spDam = HeroesManager.ReduceDamageByDef(args.magicDamage, stats.MagicalResist.Val);
            if (LogDamage)
            {
                CLog.Log($"[{_view.gameObject.name}] Took Damage {physDam}, {spDam}. " +
                         $"Defence: {HeroesManager.GetDef(stats.PhysicalResist.Val)}, {HeroesManager.GetDef(stats.MagicalResist.Val)}");
            }
            var shield = stats.Shield;
            var health = stats.HealthCurrent.Get();
            var didDamageShield = false;
            if(physDam > 0)
            {
                var startHealth = health;
                if (stats.Shield > 0)
                {
                    shield -= physDam;
                    didDamageShield = true;
                    if (shield < 0)
                    {
                        shield = 0;
                        health += shield; // left over damage (negative value)
                    }
                }
                else
                    health -= physDam;

                physDam = (startHealth - health);
                ServiceLocator.Get<IDamageDisplay>().ShowAtScreenPos((int)physDam, EDamageType.Physical, _view.heroUI.DamagePoint.position);
            }
            
            if(spDam > 0)
            {
                var startHealth = health;
                if (stats.Shield > 0)
                {
                    shield -= spDam;
                    didDamageShield = true;
                    if (shield < 0)
                    {
                        shield = 0;
                        health += shield; // left over damage
                    }
                }
                else
                    health -= spDam;
                spDam = (startHealth - health);
                ServiceLocator.Get<IDamageDisplay>().ShowAtScreenPos((int)spDam, EDamageType.Magical, _view.heroUI.DamagePoint.position);
            }
            
            if(didDamageShield)
                stats.Shield = shield;
            if (health < 0)
                health = 0;
            stats.HealthCurrent.Val = health;
            
            if(_view.Flicker != null)
                _view.Flicker.Flick();
            // CLog.Log($"[{_view.gameObject.name}] Damaged. Phys: {args.physDamage}. Magic: {args.magicDamage}");
            if (health <= 0)
            {
                if(_view.killProcessor != null)
                    _view.killProcessor.OnKilled();
                else
                    CLog.LogRed($"[{nameof(HeroHealthManager)}] IKillProcessor is null {_view.name}");
            }
            OnAfterDamage?.Invoke();
            return new DamageArgs(physDam, spDam, default);
        }


        public void SetDamageable(bool damageable)
        {
            _isDamageable = damageable;
        }

    }
}