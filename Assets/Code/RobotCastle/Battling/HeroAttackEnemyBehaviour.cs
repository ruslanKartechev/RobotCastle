﻿using System;
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

        private enum EAttackLogicStep { Waiting, Moving, Rotating, Attacking }
        
        private IHeroController _hero;
        private CancellationTokenSource _mainToken;
        private CancellationTokenSource _subToken;
        private HeroRangeCoverCheck _rangeCoverCheck;
        private EAttackLogicStep _logicStep;
        private bool _isActivated;
        private HeroStateData myState => _hero.View.state;
        private HeroStateData enemyState => myState.attackData.CurrentEnemy.View.state;
        
        private IHeroController enemy
        {
            get => _hero.View.state.attackData.CurrentEnemy;
            set => _hero.View.state.attackData.CurrentEnemy = value;
        }

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
            _hero.Battle.AttackPositionCalculator.AddUnit(_hero.View.state);
            _hero.View.attackManager.OnAttackStep += OnAttackCallback;
            _mainToken = new CancellationTokenSource();
            _hero.View.stats.CheckManaFull();
            SearchAndAttack(_mainToken.Token);
        }

        public void Stop()
        {
            _hero.View.attackManager.OnAttackStep -= OnAttackCallback;
            _logicStep = EAttackLogicStep.Waiting;
            // CLog.Log($"[{nameof(HeroAttackEnemyBehaviour)}] [Stop]");
            if(_mainToken != null)
                _mainToken.Cancel();
            if (_subToken != null)
                _subToken.Cancel();
            _hero.View.attackManager.Stop();
            _hero.Battle.AttackPositionCalculator.RemoveUnit(_hero.View.state);
            myState.SetTargetCellToSelf();
        }

        // 1 Choose target
        // 2 go to target
        // 3 attack target
        // while going  - check position
        private async void SearchAndAttack(CancellationToken token)
        {
            myState.attackData.IsMovingForDuel = false;
            myState.SetTargetCellToSelf();
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
            // var bestTarget = BattleManager.GetBestTargetForAttack(_hero);
            // if (bestTarget != null && enemy != bestTarget)
            // {
            //     // CLog.LogRed($"[{_hero.gameObject.name}] New enemy Set!");
            //     enemy = bestTarget;
            //     attackData.IsMovingForDuel = false;
            // }
            if (enemy.IsDead)
            {
                _mainToken.Cancel();
                _mainToken = new CancellationTokenSource();
                SearchAndAttack(_mainToken.Token);
                return;
            }
            if (token.IsCancellationRequested) return;
            var actionCall = CheckNextAction(enemy);
            if (_hero.View.gameObject.name.Contains("shelda"))
            {
                CLog.LogGreen($"action call: {actionCall}. State: {_logicStep.ToString()}");
            }
            switch (actionCall)
            {
                case 0: // should not move, straight to attack
                    if (_logicStep == EAttackLogicStep.Attacking)
                        return;
                    StopSubProc();
                    // movement.OnMovementStopped();
                    _logicStep = EAttackLogicStep.Attacking;
                    BeginAttackAndCheckIfDead(_subToken.Token);
                    return;
                case 1: // should move to target cell
                    if (_logicStep == EAttackLogicStep.Attacking)
                    {
                        StopSubProc();
                        StopAttack();
                    }
                    _logicStep = EAttackLogicStep.Waiting;
                    myState.SetTargetCellToSelf();
                    
                    var shouldMove = _hero.Battle.AttackPositionCalculator.GetPositionToAttack(
                        _hero, enemy, out var attackCell, out var distance);
                    if (!shouldMove)
                    {
                        CLog.LogRed($"[DecideNextStep] Wierd case");
                        await Task.Yield();
                        if (token.IsCancellationRequested) return;
                        DecideNextStep(_mainToken.Token);
                        return;
                    }
                    CLog.LogYellow($"[{_hero.View.gameObject.name}] DuelDistance {distance <= HeroesConstants.DuelMaxDistance} (dist: {distance}. max: {HeroesConstants.DuelMaxDistance})" +
                                   $"\nEnemy is me: {enemyState.attackData.CurrentEnemy == _hero}, Enemy state: {enemyState.GetStr()}");
                    if (distance <= HeroesConstants.DuelMaxDistance 
                        && enemyState.attackData.CurrentEnemy == _hero 
                        && enemyState.isMoving)
                    {
                        CLog.LogGreen($"[{_hero.View.gameObject.name}] Duel case! Enemy: {enemyState.attackData.IsMovingForDuel}.");
                        if (enemyState.attackData.IsMovingForDuel)
                        {
                            myState.attackData.IsMovingForDuel = false;
                            for(var i = 0; i < 3; i++)
                                await Task.Yield();
                            if (token.IsCancellationRequested)
                                return;
                            DecideNextStep(_mainToken.Token);
                            return;
                        }
                        else
                            myState.attackData.IsMovingForDuel = true;
                    }
                    _logicStep = EAttackLogicStep.Moving;
                    myState.TargetCell = attackCell;
                    StopSubProc();
                    WaitingForMovement(attackCell, _subToken.Token);
                    return;
                case 2:
                    StopSubProc();
                    if (_logicStep == EAttackLogicStep.Attacking)
                        StopAttack();
                    _logicStep = EAttackLogicStep.Rotating;
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
                    break;
                case EPathMovementResult.FailedToBuild:
                    CLog.LogRed($"[{_hero.View.gameObject.name}] FailedToBuild moving to {targetCell.ToString()}");
                    _hero.View.movement.OnMovementStopped();
                    await Task.Delay(waitTimeMs, token);
                    break;
            }
            DecideNextStep(_mainToken.Token);
        }
        
        /// <summary>
        /// </summary>
        /// <returns>0 - can attack. 1 - need to move, 2 - need to rotate </returns>
        private int CheckNextAction(IHeroController otherHero)
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
            _hero.View.attackManager.BeginAttack(enemy.View.damageReceiver);
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