using System;
using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroHealthManager : IHeroHealthManager, IHeroIDamageReceiver
    {
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
        
        public void TakeDamage(DamageArgs args)
        {
            OnDamageAttempted?.Invoke();
            if (!_isDamageable)
                return;
            OnDamagePreApplied?.Invoke();
            var stats = _view.stats;
            args.physDamage = HeroesManager.ReduceDamageByDef(args.physDamage, stats.PhysicalResist.Val);
            args.magicDamage = HeroesManager.ReduceDamageByDef(args.magicDamage, stats.MagicalResist.Val);
            CLog.Log($"[{_view.gameObject.name}] Took Damage {args.physDamage}, {args.magicDamage}. " +
                     $"Defence: {HeroesManager.GetDef(stats.PhysicalResist.Val)}, {HeroesManager.GetDef(stats.MagicalResist.Val)}");
            var shield = stats.Shield;
            var health = stats.HealthCurrent.Get();
            var didDamageShield = false;
            if(args.physDamage > 0)
            {
                if (stats.Shield > 0)
                {
                    shield -= args.physDamage;
                    didDamageShield = true;
                    if (shield < 0)
                    {
                        shield = 0;
                        health += shield; // left over damage
                    }
                }
                else
                    health -= args.physDamage;
                ServiceLocator.Get<IDamageDisplay>().ShowAtScreenPos((int)args.physDamage, EDamageType.Physical, _view.heroUI.DamagePoint.position);
            }
            if(args.magicDamage > 0)
            {
                if (stats.Shield > 0)
                {
                    shield -= args.magicDamage;
                    didDamageShield = true;
                    if (shield < 0)
                    {
                        shield = 0;
                        health += shield; // left over damage
                    }
                }
                else
                    health -= args.magicDamage;
                ServiceLocator.Get<IDamageDisplay>().ShowAtScreenPos((int)args.magicDamage, EDamageType.Magical, _view.heroUI.DamagePoint.position);
            }
            if(didDamageShield)
                stats.Shield = shield;
            stats.HealthCurrent.Val = health;
            
            // CLog.Log($"[{_view.gameObject.name}] Damaged. Phys: {args.physDamage}. Magic: {args.magicDamage}");
            if (health <= 0)
            {
                if(_view.killProcessor != null)
                    _view.killProcessor.OnKilled();
                else
                    CLog.LogRed($"[{nameof(HeroHealthManager)}] IKillProcessor is null {_view.name}");
            }
            OnAfterDamage?.Invoke();
        }


        public void SetDamageable(bool damageable)
        {
            _isDamageable = damageable;
        }

    }
}