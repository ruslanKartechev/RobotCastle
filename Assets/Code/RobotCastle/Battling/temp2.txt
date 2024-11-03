using System;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroAttackEnemyBehaviour : IHeroBehaviour
    {
        private enum EAttackLogicStep { Waiting, Moving, Rotating, Attacking }

        public string BehaviourID => "hero_attack";
        
        public void Activate(IHeroController hero, Action<IHeroBehaviour> endCallback)
        {
            if (_isActivated)
            {
                CLog.LogError($"[{nameof(HeroAttackEnemyBehaviour)}] [Activate] Already activated");
                return;
            }
            _isActivated = true;
            _hero = hero;
            if (_rangeCoverCheck == null)
                _rangeCoverCheck = new HeroRangeCoverCheck(_hero);
            _hero.Battle.AttackPositionCalculator.AddUnit(_hero.Components.state);
            _mainToken = new CancellationTokenSource();
            _hero.Components.stats.CheckManaFull();
            SearchAndAttack(_mainToken.Token);
        }

        public void Stop()
        {
            _isActivated = false;
            _logicStep = EAttackLogicStep.Waiting;
            // CLog.Log($"[{nameof(HeroAttackEnemyBehaviour)}] [Stop]");
            if(_mainToken != null)
                _mainToken.Cancel();
            if (_subToken != null)
                _subToken.Cancel();
            _hero.Components.attackManager.Stop();
            _hero.Battle.AttackPositionCalculator.RemoveUnit(_hero.Components.state);
            myState.SetTargetCellToSelf();
        }
        
        
        private const float DelayAfterTargetDiedMs = 300;
        private const int TimeWaitIfPathFailedMs = 250;

        private IHeroController _hero;
        private CancellationTokenSource _mainToken;
        private CancellationTokenSource _subToken;
        private HeroRangeCoverCheck _rangeCoverCheck;
        private EAttackLogicStep _logicStep;
        private bool _isActivated;
        
        private HeroStateData myState => _hero.Components.state;
        private HeroStateData enemyState => myState.attackData.CurrentEnemy.Components.state;
        private HeroMovementManager movement => _hero.Components.movement;
        private IHeroController enemy
        {
            get => _hero.Components.state.attackData.CurrentEnemy;
            set => _hero.Components.state.attackData.CurrentEnemy = value;
        }

        
        // 1 Choose target
        // 2 go to target
        // 3 attack target
        // while going  - check position
        private async void SearchAndAttack(CancellationToken token)
        {
            if (!_isActivated)
                return;
            myState.attackData.IsMovingForDuel = false;
            myState.SetTargetCellToSelf();
            _rangeCoverCheck.Update(_hero.Components.transform.position, true);
            var enemies = BattleManager.GetBestTargetForAttack(_hero, enemy);
            while ((enemies == null || enemies.Count == 0) && !token.IsCancellationRequested)
            {
                const int waitTimMs = 500;
                CLog.Log($"[{_hero.Components.gameObject.name}] Closest enemy is null. Waiting {waitTimMs} ms.");
                await Task.Delay(waitTimMs, token);
                enemies = BattleManager.GetBestTargetForAttack(_hero, enemy);
            }
            if (token.IsCancellationRequested)
                return;
            DecideNextStep(token);
        }
        
        private async void DecideNextStep(CancellationToken token)
        {
            await Task.Yield();
            if (token.IsCancellationRequested)
                return;
            var targets= BattleManager.GetBestTargetForAttack(_hero, enemy);
            while(targets.Count == 0)
            {
                CLog.LogRed("No targets, waiting");
                targets = BattleManager.GetBestTargetForAttack(_hero, enemy);
                await Task.Delay(250, token);
            }
            var prevStep = _logicStep;
            _logicStep = EAttackLogicStep.Waiting;
            StopSubProc();
            _rangeCoverCheck.Update(_hero.Components.state.currentCell, false);
            foreach (var tempEnemy in targets)
            {
                if (tempEnemy.IsDead)
                    continue;
                var inRange = _rangeCoverCheck.IsHeroWithinRange(tempEnemy);
                if (inRange)
                {
                    var shallRot = movement.CheckIfShouldRotate(tempEnemy.Components.state.currentCell);
                    if (shallRot)
                    {
                        if (prevStep == EAttackLogicStep.Attacking)
                        {
                            // CLog.LogRed("prevStep == EAttackLogicStep.Attacking");
                            _hero.Components.attackManager.Stop();
                        }
                        _logicStep = EAttackLogicStep.Rotating;
                        movement.RotateIfNecessary(tempEnemy.Components.state.currentCell, _subToken.Token, OnRotated);
                        return;
                    }
                    else
                    {
                        enemy = tempEnemy;
                        BeginAttack(prevStep, _subToken.Token);
                        return;
                    }
                }

                var (canMove, point) = await CheckIfCanMoveToEnemy(tempEnemy, token);
                if(token.IsCancellationRequested) return;
                if (!canMove)
                {
                    await Task.Yield();
                    if (token.IsCancellationRequested) return;
                    continue;
                }
                    
                if (prevStep == EAttackLogicStep.Attacking)
                {
                    _hero.Components.attackManager.Stop();
                }
                enemy = tempEnemy;
                var enState = enemy.Components.state;
                // var distance = (point - myState.currentCell).sqrMagnitude <= HeroesConstants.DuelMaxDistance;
                var distance = (myState.currentCell - tempEnemy.Components.state.currentCell).sqrMagnitude; 
                if (distance < HeroesConstants.DuelMaxDistance
                    && enState.attackData.CurrentEnemy == _hero 
                    && enState.isMoving)
                {
                    if (enState.attackData.IsMovingForDuel)
                    {
                        // CLog.LogYellow("Waiting for duel case!");
                        myState.attackData.IsMovingForDuel = false;
                        AwaitUntilEnemyStartsAttacking(_subToken.Token);
                        return;
                    }
                    myState.attackData.IsMovingForDuel = true;
                }
                        
                Move(point, prevStep, _subToken.Token);
                return;
            }
            CLog.LogRed($"Error! No enemies could do anything");
            await Task.Delay(250, token);
            DecideNextStep(token);
        }

        private async void AwaitUntilEnemyStartsAttacking(CancellationToken token)
        {
            var enState = enemy.Components.state;
            await Task.Yield();
            myState.SetTargetCellToSelf();
            movement.OnMovementStopped();
            while (token.IsCancellationRequested == false
                   && enemy.IsDead == false
                   && enState.isAttacking == false
                   && enState.isMoving
                   && enState.attackData.CurrentEnemy == _hero)
            {
                await Task.Yield();
            }

            if (token.IsCancellationRequested)
                return;
            DecideNextStep(_mainToken.Token);
        }
        
        private async void BeginAttack(EAttackLogicStep prevStep, CancellationToken token)
        {
            if (prevStep == EAttackLogicStep.Attacking)
            {
                _hero.Components.attackManager.Stop();
            }

            myState.SetTargetCellToSelf();
            movement.OnMovementStopped();
            _logicStep = EAttackLogicStep.Attacking;
            await BeginAttackAndCheckIfDead(token);
        }

        private async Task<(bool, Vector2Int)> CheckIfCanMoveToEnemy(IHeroController enemy, CancellationToken token)
        {
            var positions = _hero.Battle.AttackPositionCalculator.GetPossibleCellsToAttackEnemy(_hero, enemy);
            if (positions.Count == 0)
            {
                CLog.LogRed($"PossiblePositions: {positions.Count}");
                return (false, default);
            }
            var myPos = myState.currentCell;
            positions.Sort((a, b) => (myPos - a).sqrMagnitude.CompareTo((myPos - b).sqrMagnitude));
            foreach (var p in positions)
            {
                var result = await _hero.Components.agent.CheckIfCanMove(p, token);
                if(result)
                    return (true, p);
            }
            // CLog.Log($"Cannot build path to any position!");
            return (false, default);
        }

        private async void Move(Vector2Int cell, EAttackLogicStep prevStep, CancellationToken token)
        {
            _hero.Components.attackManager.Stop();
            if (prevStep == EAttackLogicStep.Attacking)
            {
                _logicStep = EAttackLogicStep.Waiting;
                await Task.Delay(500, token);
            }
            if(prevStep != EAttackLogicStep.Moving)
            {
                // CLog.LogYellow("Prev step was not moving");
                movement.OnMovementBegan();
            }
            _logicStep = EAttackLogicStep.Moving;
            var result = await movement.MoveToCell(cell, token);
            if (token.IsCancellationRequested)
                return;
            switch (result)
            {
                case EPathMovementResult.IsBlockedOnTheWay:
                    // CLog.LogRed($"[{_hero.gameObject.name}] IsBlockedOnTheWay moving to {targetCell.ToString()}");
                    await Task.Delay(TimeWaitIfPathFailedMs, token);
                    break;
                case EPathMovementResult.FailedToBuild:
                    // CLog.LogRed($"[{_hero.Components.gameObject.name}] FailedToBuild moving to {attackCell.ToString()}");
                    await Task.Delay(TimeWaitIfPathFailedMs, token);
                    break;
            }
            await Task.Yield();
            if (token.IsCancellationRequested) return;
            
            DecideNextStep(_mainToken.Token);
        }
        
        private void OnRotated()
        {
            DecideNextStep(_mainToken.Token);
        }

        private void StopSubProc()
        {
            _subToken?.Cancel();
            _subToken = new CancellationTokenSource();
        }

        private async Task BeginAttackAndCheckIfDead(CancellationToken token)
        {
            _hero.Components.attackManager.BeginAttack(enemy.Components.damageReceiver);
            var startEnemyCell = enemy.Components.state.currentCell;
            while (!token.IsCancellationRequested)
            {
                if (enemy.IsDead)
                {
                    enemy.Components.attackManager.Stop();
                    DecideNextStep(_mainToken.Token);
                    return;
                }
                var currentEnemyCell = enemy.Components.state.currentCell;
                if (startEnemyCell != currentEnemyCell)
                {
                    // enemy.Components.attackManager.Stop();
                    DecideNextStep(_mainToken.Token);
                    if (token.IsCancellationRequested)
                        return;
                    startEnemyCell = currentEnemyCell;
                }
                await Task.Yield();
                if (token.IsCancellationRequested)
                    return;
            }
        }
        
        
    }
}