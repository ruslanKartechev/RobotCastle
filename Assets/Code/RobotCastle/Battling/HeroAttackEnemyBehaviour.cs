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
        
        public string BehaviourID => "hero_attack";

        private enum AttackBehaviourState { Waiting, Moving, Rotating, Attacking }
        
        private IHeroController _hero;
        private CancellationTokenSource _mainToken;
        private CancellationTokenSource _subToken;
        private HeroRangeCoverCheck _rangeCoverCheck;
        private AttackBehaviourState _state;
        private bool _isActivated;
        
        private IHeroController enemy
        {
            get => _hero.View.attackData.CurrentEnemy;
            set => _hero.View.attackData.CurrentEnemy = value;
        }

        private AttackTargetData attackData => _hero.View.attackData;
        private HeroMovementManager movement => _hero.View.movement;
        
        
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
            _hero.Battle.AttackPositionCalculator.AddUnit(_hero.View.movement);
            _hero.View.attackManager.OnAttackStep += OnAttackCallback;
            _mainToken = new CancellationTokenSource();
            _hero.View.stats.CheckManaFull();
            SearchAndAttack(_mainToken.Token);
        }

        public void Stop()
        {
            _hero.View.attackManager.OnAttackStep -= OnAttackCallback;
            _state = AttackBehaviourState.Waiting;
            // CLog.Log($"[{nameof(HeroAttackEnemyBehaviour)}] [Stop]");
            if(_mainToken != null)
                _mainToken.Cancel();
            if (_subToken != null)
                _subToken.Cancel();
            _hero.View.agent.SetCellMoveCheck(null);
            _hero.View.attackManager.Stop();
            _hero.Battle.AttackPositionCalculator.RemoveUnit(_hero.View.movement);
            _hero.View.movement.SetNullTargetCell();
        }

        // 1 Choose target
        // 2 go to target
        // 3 attack target
        // while going  - check position
        private async void SearchAndAttack(CancellationToken token)
        {
            attackData.IsMovingForDuel = false;
            movement.SetNullTargetCell();
            _rangeCoverCheck.Update(_hero.View.transform.position, true);
            enemy = BattleManager.GetBestTargetForAttack(_hero);
            while (enemy == null && !token.IsCancellationRequested)
            {
                const float waitTimeSec = 1f;
                CLog.Log($"[{_hero.View.gameObject.name}] Closest enemy is null. Waiting {waitTimeSec} sec.");
                await Task.Delay((int)(waitTimeSec * 1000f), token);
                enemy = BattleManager.GetBestTargetForAttack(_hero);
            }
            if (token.IsCancellationRequested)
                return;
            _rangeCoverCheck.Update(_hero.View.transform.position, false);
            await Task.Yield();
            if (token.IsCancellationRequested)
                return;
            DecideNextStep(token);
        }
        
        private async void DecideNextStep(CancellationToken token)
        {
            var bestTarget = BattleManager.GetBestTargetForAttack(_hero);
            if (bestTarget != null && enemy != bestTarget)
            {
                // CLog.LogRed($"[{_hero.gameObject.name}] New enemy Set!");
                enemy = bestTarget;
                attackData.IsMovingForDuel = false;
            }
            if (enemy.IsDead)
            {
                _mainToken.Cancel();
                _mainToken = new CancellationTokenSource();
                SearchAndAttack(_mainToken.Token);
                return;
            }
            if (token.IsCancellationRequested) return;
            
            _hero.View.agent.SetCellMoveCheck(OnMoveStepCallback);
            var actionCall = CheckIfShallMove(enemy);
            switch (actionCall)
            {
                case 0: // should not move, straight to attack
                    if (_state == AttackBehaviourState.Attacking)
                        return;
                    movement.OnMovementStopped();
                    StopSubProc();
                    _state = AttackBehaviourState.Attacking;
                    BeginAttackAndCheckIfDead(_subToken.Token);
                    return;
                case 1: // should move to target cell
                    if (_state == AttackBehaviourState.Attacking)
                    {
                        StopSubProc();
                        StopAttack();
                    }
                    _state = AttackBehaviourState.Waiting;
                    movement.SetNullTargetCell();
                    
                    var shouldMove = _hero.Battle.AttackPositionCalculator.GetPositionToAttack(_hero, enemy, out var enemyCell, out var distance);
                    if (!shouldMove)
                    {
                        CLog.LogRed($"[DecideNextStep] Wierd case");
                        await Task.Yield();
                        if (token.IsCancellationRequested) return;
                        DecideNextStep(_mainToken.Token);
                        return;
                    }
                    CLog.LogYellow($"[{_hero.View.gameObject.name}] DuelDistance {distance <= HeroesConfig.DuelMaxDistance} (dist: {distance}. max: {HeroesConfig.DuelMaxDistance}), Enemy is me: {enemy.View.attackData.CurrentEnemy == _hero}, Enemy is moving: {enemy.View.agent.IsMoving}");
                    if (distance <= HeroesConfig.DuelMaxDistance 
                        && enemy.View.attackData.CurrentEnemy == _hero 
                        && enemy.View.agent.IsMoving)
                    {
                        CLog.LogGreen($"[{_hero.View.gameObject.name}] Enemy Moving ForDuel: {enemy.View.attackData.IsMovingForDuel}");
                        if (enemy.View.attackData.IsMovingForDuel)
                        {
                            attackData.IsMovingForDuel = false;
                            for(var i = 0; i < 3; i++)
                                await Task.Yield();
                            if (token.IsCancellationRequested)
                                return;
                            DecideNextStep(_mainToken.Token);
                            return;
                        }
                        else
                            attackData.IsMovingForDuel = true;
                    }
                    _state = AttackBehaviourState.Moving;
                    movement.TargetCell = enemyCell;
                    StopSubProc();
                    WaitingForMovement(enemyCell, _subToken.Token);
                    return;
                case 2:
                    StopSubProc();
                    if (_state == AttackBehaviourState.Attacking)
                        StopAttack();
                    _state = AttackBehaviourState.Rotating;
                    _hero.View.movement.RotateIfNecessary(enemy.View.agent.CurrentCell, _subToken.Token, OnRotated);
                    return;
            }
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
        
        private bool OnMoveStepCallback(Vector2Int newCell)
        {
            DecideNextStep(_mainToken.Token);
            return false;
        }

        private void OnAttackCallback()
        {
            // CLog.Log($"[{_hero.gameObject.name}] OnAttackCallback");
            DecideNextStep(_mainToken.Token);
        }

        private async void WaitingForMovement(Vector2Int targetCell, CancellationToken token)
        {
            const int waitTimeMs = (int)(1000 * .25f);
            var result = await _hero.View.movement.MoveToCell(targetCell, token);
            switch (result)
            {
                case EPathMovementResult.IsBlockedOnTheWay:
                    // CLog.LogRed($"[{_hero.gameObject.name}] IsBlockedOnTheWay moving to {targetCell.ToString()}");
                    await Task.Delay(waitTimeMs, token);
                    DecideNextStep(_mainToken.Token);
                    break;
                case EPathMovementResult.FailedToBuild:
                    CLog.LogRed($"[{_hero.View.gameObject.name}] FailedToBuild moving to {targetCell.ToString()}");
                    _hero.View.movement.OnMovementStopped();
                    await Task.Delay(waitTimeMs, token);
                    DecideNextStep(_mainToken.Token);
                    break;
            }
        }
        
        /// <summary>
        /// </summary>
        /// <returns>0 - can attack. 1 - need to move, 2 - need to rotate </returns>
        private int CheckIfShallMove(IHeroController otherHero)
        {
            _rangeCoverCheck.Update(_hero.View.transform.position, true);
            var isWithin = _rangeCoverCheck.IsHeroWithinRange(otherHero);
            if (!isWithin)
                return 1;
            var shouldRotate = _hero.View.movement.CheckIfShouldRotate(enemy.View.agent.CurrentCell);
            if (shouldRotate)
                return 2;
            return 0;
        }

        private void StopAttack()
        {
            _hero.View.attackManager.Stop();
            // CLog.Log($"[{_hero.gameObject.name}] Stop Attack Behaviour");
        }
        
        private async Task BeginAttackAndCheckIfDead(CancellationToken token)
        {
            _hero.View.attackManager.BeginAttack(enemy.View.transform, enemy.View.damageReceiver);
            var startEnemyCell = enemy.View.agent.CurrentCell;
            while (!token.IsCancellationRequested)
            {
                if (enemy.IsDead)
                {
                    DecideNextStep(_mainToken.Token);
                    return;
                }
                var currentEnemyCell = enemy.View.agent.CurrentCell;
                if (startEnemyCell != currentEnemyCell)
                {
                    // This method will either stop attack or do nothing, hence continue the loop
                    DecideNextStep(_mainToken.Token);
                    if (token.IsCancellationRequested)
                        return;
                    startEnemyCell = currentEnemyCell;
                }
                if (token.IsCancellationRequested)
                    return;
                await Task.Yield();
            }
        }
        
        
    }
}