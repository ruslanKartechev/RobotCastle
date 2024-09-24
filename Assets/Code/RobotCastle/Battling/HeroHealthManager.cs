using RobotCastle.Core;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroHealthManager : MonoBehaviour, IHeroHealthManager, IHeroIDamageReceiver
    {
        [SerializeField] private HeroView _heroView;
        private bool _isDamageable = true;

        public bool IsDamageable => _isDamageable;
        
        public void TakeDamage(DamageArgs args)
        {
            if (!_isDamageable)
                return;
            var amount = args.amount;
            amount *= (1f - _heroView.Stats.PhysicalResist.Val);
            if (amount < 0)
            {
                return;
            }
            // CLog.Log($"{gameObject.name} Damaged taken. {args.type}. Amount {amount}. Resist {_heroView.Stats.PhysicalResist.Val}");
            _heroView.Stats.HealthCurrent.Val -= amount;
            var pos = transform.position + Vector3.up;
            ServiceLocator.Get<IDamageDisplay>().ShowAt((int)amount, args.type, pos);
            // var health = _heroView.Stats.HealthCurrent.Val;
            // var t = health / _heroView.Stats.HealthMax.Val;
            if (_heroView.Stats.HealthCurrent.Val <= 0)
            {
                // CLog.Log($"{gameObject.name} Health < 0. Trying to die");
                var killProcessor = gameObject.GetComponent<IKillProcessor>();
                if(killProcessor != null)
                    killProcessor.OnKilled();
                else
                {
                    CLog.LogRed($"[{nameof(HeroHealthManager)}] IKillProcessor is null {gameObject.name}");
                }
            }
        }

        public void SetDamageable(bool damageable)
        {
            _isDamageable = damageable;
        }


    }
}