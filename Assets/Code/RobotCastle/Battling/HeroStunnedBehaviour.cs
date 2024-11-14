using System;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroStunnedBehaviour : IHeroBehaviour
    {
        public HeroStunnedBehaviour(float duration)
        {
            _duration = duration;
            _particles = ServiceLocator.Get<ISimplePoolsManager>().GetOne(ObjectPoolConstants.FxPoolId_Stunned);
        }
        
        public string BehaviourID => "stunned";
        
        private Action<IHeroBehaviour> _callback;
        private IHeroController _hero;
        private IPoolItem _particles;
        private float _duration;
        private CancellationTokenSource _token;
        
        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            CLog.Log($"[{nameof(HeroStunnedBehaviour)}] Activated on: {hero.Components.gameObject.name}");

            _hero = hero;
            _callback = endCallback;
            _hero.Components.movement.Stop();
            _hero.Components.attackManager.Stop();
            _hero.Components.animator.Play("Stunned", 0, 0);
            _particles.GetGameObject().transform.position = _hero.Components.transform.position + Vector3.up * _hero.Components.StunnedFxHeight;
            _particles.PoolShow();
            _token = new CancellationTokenSource();
            Wait(_token.Token);
        }

        public void Stop()
        {
            _hero.Components.animator.Play("Idle", 0, 0);
            _hero.Components.state.SetStunned(false);
            _token?.Cancel();
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(_particles);
        }

        private async void Wait(CancellationToken token)
        {
            _hero.Components.state.SetStunned(true);
            await Task.Delay((int)(_duration * 1000), token);
            if (token.IsCancellationRequested) return;
            _hero.Components.animator.Play("Idle", 0, 0);
            _hero.Components.state.SetStunned(false);
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(_particles);
            _callback?.Invoke(this);
        }
        
    }
}