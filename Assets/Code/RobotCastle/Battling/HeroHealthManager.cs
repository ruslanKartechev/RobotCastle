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
            args.physDamage -= _view.Stats.PhysicalResist.Val;
            args.magicDamage -= _view.Stats.MagicalResist.Val;
            if (args.physDamage <= 0)
            {
                args.physDamage = 0;
            }
            else
            {
                _view.Stats.HealthCurrent.Val -= args.physDamage;
                ServiceLocator.Get<IDamageDisplay>().ShowAt((int)args.physDamage, EDamageType.Physical, _view.transform.position + Vector3.up);
            }
            if (args.magicDamage <= 0)
            {
                args.magicDamage = 0;
            }
            else
            {
                ServiceLocator.Get<IDamageDisplay>().ShowAt((int)args.magicDamage, EDamageType.Magical, _view.transform.position + Vector3.up);
            }
            // CLog.Log($"[{_view.gameObject.name}] Damaged. Phys: {args.physDamage}. Magic: {args.magicDamage}");
            if (_view.Stats.HealthCurrent.Val <= 0)
            {
                if(_view.KillProcessor != null)
                    _view.KillProcessor.OnKilled();
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