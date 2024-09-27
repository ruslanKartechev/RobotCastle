using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroHealthManager : IHeroHealthManager, IHeroIDamageReceiver
    {
        public HeroHealthManager(HeroView heroView)
        {
            _view = heroView;
            
        }
        
        public bool IsDamageable => _isDamageable;
        
        private HeroView _view;
        private bool _isDamageable = true;
        
        public void TakeDamage(DamageArgs args)
        {
            if (!_isDamageable)
                return;
            args.physDamage = HeroesHelper.ReduceDamageByDef(args.physDamage, _view.stats.PhysicalResist.Val);
            args.magicDamage = HeroesHelper.ReduceDamageByDef(args.magicDamage, _view.stats.MagicalResist.Val);
            if(args.physDamage > 0)
            {
                _view.stats.HealthCurrent.Val -= args.physDamage;
                ServiceLocator.Get<IDamageDisplay>().ShowAt((int)args.physDamage, EDamageType.Physical, _view.transform.position + Vector3.up);
            }
            if(args.magicDamage > 0)
            {
                _view.stats.HealthCurrent.Val -= args.magicDamage;
                ServiceLocator.Get<IDamageDisplay>().ShowAt((int)args.magicDamage, EDamageType.Magical, _view.transform.position + Vector3.up);
            }
            // CLog.Log($"[{_view.gameObject.name}] Damaged. Phys: {args.physDamage}. Magic: {args.magicDamage}");
            if (_view.stats.HealthCurrent.Val <= 0)
            {
                if(_view.killProcessor != null)
                    _view.killProcessor.OnKilled();
                else
                    CLog.LogRed($"[{nameof(HeroHealthManager)}] IKillProcessor is null {_view.name}");
            }
        }

        public void SetDamageable(bool damageable)
        {
            _isDamageable = damageable;
        }


    }
}