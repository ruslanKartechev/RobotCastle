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
        private enum AttackBehaviourState
        {
            Waiting, Moving, Rotating, Attacking
        }
        
        private HeroController _hero;

        private CancellationTokenSource _mainToken;
        private CancellationTokenSource _subToken;
        private HeroRangeCoverCheck _rangeCoverCheck;
        private AttackBehaviourState _state;
        private bool _isActivated;
        private Action<IHeroBehaviour> _callback;
        
        private HeroController enemy
        {
            get => _hero.HeroView.AttackInfo.CurrentEnemy;
            set => _hero.HeroView.AttackInfo.CurrentEnemy = value;
        }
        
        public string BehaviourID => "hero_attack";
        
        public void Activate(GameObject target, Action<IHeroBehaviour> endCallback)
        {
            var hero = target.GetComponent<HeroController>();
            Activate(hero, endCallback);
        }

        public void Activate(HeroController hero, Action<IHeroBehaviour> endCallback)
        {
            if (_isActivated)
            {
                CLog.LogError($"[{nameof(HeroAttackEnemyBehaviour)}] [Activate] Already activated");
                return;
            }
            _isActivated = true;
            _hero = hero;
            _callback = endCallback;
            if (_rangeCoverCheck == null)
                _rangeCoverCheck = new HeroRangeCoverCheck(_hero);
            _hero.Battle.AttackPositionCalculator.AddUnit(_hero.HeroView.movement);
            _hero.HeroView.AttackManager.OnAttackStep += OnAttackCallback;
            
            if(_mainToken != null)
                _mainToken.Cancel();
            _mainToken = new CancellationTokenSource();
            SearchAndAttack(_mainToken.Token);
        }

        public void Stop()
        {
            _hero.HeroView.AttackManager.OnAttackStep -= OnAttackCallback;
            _state = AttackBehaviourState.Waiting;
            // CLog.Log($"[{nameof(HeroAttackEnemyBehaviour)}] [Stop]");
            if(_mainToken != null)
                _mainToken.Cancel();
            if (_subToken != null)
                _subToken.Cancel();
            _hero.HeroView.agent.SetCellMoveCheck(null);
            _hero.HeroView.AttackManager.Stop();
            _hero.Battle.AttackPositionCalculator.RemoveUnit(_hero.HeroView.movement);
            _hero.HeroView.movement.SetNullTargetCell();
        }

        // 1 Choose target
        // 2 go to target
        // 3 attack target
        // while going  - check position
        private async void SearchAndAttack(CancellationToken token)
        {
            _hero.HeroView.AttackInfo.IsMovingForDuel = false;
            _rangeCoverCheck.Update(_hero.transform.position, true);
            enemy = BattleManager.GetBestTargetForAttack(_hero);
            while (enemy == null && !token.IsCancellationRequested)
            {
                const float delay = 1f;
                CLog.Log($"[{_hero.gameObject.name}] Closest enemy is null. Waiting {delay} sec.");
                await Task.Delay((int)(delay * 1000f), token);
                enemy = BattleManager.GetBestTargetForAttack(_hero);
            }
            if (token.IsCancellationRequested)
                return;
            _rangeCoverCheck.Update(_hero.transform.position, false);
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
                _hero.HeroView.AttackInfo.IsMovingForDuel = false;
            }
            if (enemy.IsDead)
            {
                await Task.Yield();
                if (token.IsCancellationRequested) return;
            
                _mainToken.Cancel();
                _mainToken = new CancellationTokenSource();
                SearchAndAttack(_mainToken.Token);
                return;
            }
            if (token.IsCancellationRequested) return;
            
            _hero.HeroView.agent.SetCellMoveCheck(OnMoveStepCallback);
            var actionCall = CheckIfShallMove(enemy);
            switch (actionCall)
            {
                case 0: // should not move, straight to attack
                    if (_state == AttackBehaviourState.Attacking)
                        return;
                    _hero.HeroView.movement.OnMovementStopped();
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
                    _hero.HeroView.movement.SetNullTargetCell();
                    
                    var shouldMove = _hero.Battle.AttackPositionCalculator.GetPositionToAttack(_hero, enemy, out var enemyCell, out int distance);
                    if (!shouldMove)
                    {
                        CLog.LogRed($"[DecideNextStep] Wierd case");
                        await Task.Yield();
                        if (token.IsCancellationRequested) return;
                        
                        DecideNextStep(_mainToken.Token);
                        return;
                    }
                    
                    if (distance <= HeroesConfig.DuelMaxDistance && enemy.HeroView.AttackInfo.CurrentEnemy == _hero)
                    {
                        if (enemy.HeroView.AttackInfo.IsMovingForDuel)
                        {
                            for(var i = 0; i < 3; i++)
                                await Task.Yield();
                            if (token.IsCancellationRequested)
                                return;
                            DecideNextStep(_mainToken.Token);
                            return;
                        }
                        else
                            _hero.HeroView.AttackInfo.IsMovingForDuel = true;
                    }
                    _state = AttackBehaviourState.Moving;
                    _hero.HeroView.movement.TargetCell = enemyCell;
                    StopSubProc();
                    WaitingForMovement(enemyCell, _subToken.Token);
                    return;
                case 2:
                    StopSubProc();
                    if (_state == AttackBehaviourState.Attacking)
                        StopAttack();
                    _state = AttackBehaviourState.Rotating;
                    _hero.HeroView.movement.RotateIfNecessary(enemy.HeroView.agent.CurrentCell, _subToken.Token, OnRotated);
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
            const int waitTime = (int)(1000 * .5f);
            var result = await _hero.HeroView.movement.MoveToCell(targetCell, token);
            switch (result)
            {
                case EPathMovementResult.IsBlockedOnTheWay:
                    CLog.LogRed($"[{_hero.gameObject.name}] IsBlockedOnTheWay moving to {targetCell.ToString()}");
                    await Task.Delay(waitTime, token);
                    DecideNextStep(_mainToken.Token);
                    break;
                case EPathMovementResult.FailedToBuild:
                    CLog.LogRed($"[{_hero.gameObject.name}] FailedToBuild moving to {targetCell.ToString()}");
                    await Task.Delay(waitTime, token);
                    DecideNextStep(_mainToken.Token);
                    break;
            }
        }
        
        /// <summary>
        /// </summary>
        /// <returns>0 - can attack. 1 - need to move, 2 - need to rotate </returns>
        private int CheckIfShallMove(HeroController enemy)
        {
            _rangeCoverCheck.Update(_hero.transform.position, true);
            var isWithin = _rangeCoverCheck.IsHeroWithinRange(enemy);
            if (!isWithin)
                return 1;
            var shouldRotate = _hero.HeroView.movement.CheckIfShouldRotate(enemy.HeroView.agent.CurrentCell);
            if (shouldRotate)
                return 2;
            return 0;
        }

        private void StopAttack()
        {
            _hero.HeroView.AttackManager.Stop();
            // CLog.Log($"[{_hero.gameObject.name}] Stop Attack Behaviour");
        }
        
        private async Task BeginAttackAndCheckIfDead(CancellationToken token)
        {
            _hero.HeroView.AttackManager.BeginAttack(enemy.transform, enemy.HeroView.DamageReceiver);
            var startEnemyCell = enemy.HeroView.agent.CurrentCell;
            while (!token.IsCancellationRequested)
            {
                if (enemy.IsDead)
                {
                    DecideNextStep(_mainToken.Token);
                    return;
                }
                var currentEnemyCell = enemy.HeroView.agent.CurrentCell;
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