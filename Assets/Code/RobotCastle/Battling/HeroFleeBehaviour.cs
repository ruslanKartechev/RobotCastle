using System;
using System.Threading;
using System.Threading.Tasks;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroFleeBehaviour : IHeroBehaviour, IStatDecorator
    {
        public HeroFleeBehaviour(float time, float speedMod = .5f)
        {
            _time = time;
            _speedMod = speedMod;
        }
        
        public string BehaviourID => "pushback";
        
        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            CLog.Log($"Started {nameof(HeroFleeBehaviour)} on {hero.Components.gameObject.name}");
            _hero = hero;
            _components = hero.Components;
            _callback = endCallback;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _isActive = true;
            Working(_token.Token);
        }

        public void Stop()
        {
            if (_isActive)
            {
                _isActive = false;
                _components.stats.MoveSpeed.RemoveDecorator(this);
                _token?.Cancel();
            }
        }
        
        public string name => "fleeing";
        public int order => 50;
        public float Decorate(float val) => val * _speedMod;
        
        private float _time;
        private float _speedMod;
        private CancellationTokenSource _token;
        private bool _isActive;
        private IHeroController _hero;
        private HeroComponents _components;
        private Action<IHeroBehaviour> _callback;

        private async void Working(CancellationToken token)
        {
            _components.stats.MoveSpeed.AddDecorator(this);
            var elapsed = 0f;
            var currentCell = _components.state.currentCell;
            var targetCell = new Vector2Int(currentCell.x, currentCell.y - 1);
            if (!_components.movement.Map.IsOutOfBounce(targetCell))
                _components.movement.MoveToCell(targetCell);
            do
            {
                var newCurrentCell = _components.state.currentCell;
                if (currentCell != newCurrentCell)
                {
                    currentCell = newCurrentCell;
                    targetCell = new Vector2Int(currentCell.x, currentCell.y - 1);
                    if (!_components.movement.Map.IsOutOfBounce(targetCell))
                        _components.movement.MoveToCell(targetCell);
                }
                elapsed += Time.deltaTime;
                await Task.Yield();
            } while (!token.IsCancellationRequested && elapsed < _time);
            if(token.IsCancellationRequested) return;
            
            _components.movement.Stop();
            _components.stats.MoveSpeed.RemoveDecorator(this);
            _callback?.Invoke(this);
        }

    }
}