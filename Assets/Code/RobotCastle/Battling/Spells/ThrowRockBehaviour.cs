using System;
using System.Threading;
using System.Threading.Tasks;
using SleepDev;

namespace RobotCastle.Battling
{
    public class ThrowRockBehaviour : IHeroBehaviour, IAttackHitAction
    {
        public ThrowRockBehaviour(int range, float stunDuration)
        {
            _squareRange = range;
            _stunDuration = stunDuration;
            var r = new AttackRangeRectangle(range, range).GetCellsMask();
            _mask = new CellsMask(r);
        }

        public string BehaviourID => "throw_rock";
        
        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            _endCallback = endCallback;
            _hero = hero;
            _components = hero.Components;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            _prevHitAction = _hero.Components.attackManager.HitAction;
            _hero.Components.attackManager.HitAction = this;
            Working(_token.Token);
        }

        public void Stop()
        {
            _token?.Cancel();
            _hero.Components.attackManager.HitAction = _prevHitAction;
        }
        
        private int _squareRange;
        private float _stunDuration;
        private IHeroController _hero;
        private HeroComponents _components;
        private Action<IHeroBehaviour> _endCallback;
        private CancellationTokenSource _token;
        private CellsMask _mask;
        private IAttackHitAction _prevHitAction;
        
        private IHeroController _currentEnemy;
        private IHeroController currentEnemy
        {
            get => _currentEnemy;
            set
            {
                _currentEnemy = value;
                _components.state.attackData.CurrentEnemy = value;
            }
        }
      
        private async void Working(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!IsTargetValid())
                {
                    var didSet = SetFurthestEnemy();
                    CLog.LogRed($"Did set: {didSet}");
                    if (!didSet)
                    {
                        await Task.Yield();
                        if (token.IsCancellationRequested)
                            return;
                        continue;
                    }
                    _components.attackManager.BeginAttack(currentEnemy.Components.damageReceiver);
                }
                
                await Task.Yield();
            }
        }

        private bool IsTargetValid() => _currentEnemy is { IsDead: false };

        private bool SetFurthestEnemy()
        {
            var enemies = HeroesManager.GetHeroesEnemies(_components);
            if (enemies.Count == 0)
            {
                _currentEnemy = null;
                return false;
            }
            var minD2 = 0;
            var mPos = _components.state.currentCell;
            IHeroController result = null;
            foreach (var en in enemies)
            {
                var d2 = (mPos - en.Components.state.currentCell).sqrMagnitude;
                if (d2 >= minD2)
                {
                    minD2 = d2;
                    result = en;
                }
            }
            if (result == null)
            {
                _currentEnemy = null;
                return false;
            }
            currentEnemy = result;
            return true;
        }

        public void Hit(object target)
        {
            CLog.Log($"Hit =============");
            var dm = (IDamageReceiver)target;
            if (dm == null)
                return;
            var mainEnemy = dm.GetGameObject().GetComponent<IHeroController>();
            var enemies = HeroesManager.GetHeroesEnemies(_components);
            var hitEnemies = HeroesManager.GetHeroesInsideCellMask(_mask, mainEnemy.Components.state.currentCell, _components.movement.Map, enemies);
            foreach (var en in hitEnemies)
            {
                if (en.IsDead)
                    continue;
                _components.damageSource.DamagePhys(en.Components.damageReceiver);
                en.SetBehaviour(new HeroStunnedBehaviour(_stunDuration));
            }
            
        }
    }
}