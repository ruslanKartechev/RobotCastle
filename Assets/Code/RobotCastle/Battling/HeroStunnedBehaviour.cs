using System;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroStunnedBehaviour : IHeroBehaviour
    {
        public string BehaviourID => "stunned";
        
        private Action<IHeroBehaviour> _callback;
        private IHeroController _hero;
        private IPoolItem _particles;
        private float _duration;
        private CancellationTokenSource _token;

        public HeroStunnedBehaviour(float duration)
        {
            _duration = duration;
            _particles = ServiceLocator.Get<ISimplePoolsManager>().GetOne(ObjectPoolConstants.FxPoolId_Stunned);
        }
        
        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            _hero = hero;
            _callback = endCallback;
            _hero.Components.movement.Stop();
            _hero.Components.attackManager.Stop();
            _hero.Components.animator.Play("Stunned");
            _particles.GetGameObject().transform.position = _hero.Components.transform.position + Vector3.up * _hero.Components.StunnedFxHeight;
            _particles.PoolShow();
            _token = new CancellationTokenSource();
            Wait(_token.Token);
        }

        public void Stop()
        {
            _token?.Cancel();
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(_particles);
        }

        private async void Wait(CancellationToken token)
        {
            await Task.Delay((int)(_duration * 1000), token);
            if (token.IsCancellationRequested) return;
            ServiceLocator.Get<ISimplePoolsManager>().ReturnOne(_particles);
            _callback?.Invoke(this);
        }
        
    }
}