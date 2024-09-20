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
        private HeroController _enemy;
        private CancellationTokenSource _mainToken;
        private CancellationTokenSource _subToken;
        private HeroRangeCoverCheck _rangeCoverCheck;
        private AttackBehaviourState _state;
        private bool _isActivated;
        private Action<IHeroBehaviour> _callback;
        
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
            CLog.Log($"[{nameof(HeroAttackEnemyBehaviour)}] [Stop]");
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
            _rangeCoverCheck.Update(_hero.transform.position, true);
            _enemy = BattleManager.GetBestTargetForAttack(_hero);
            while (_enemy == null && !token.IsCancellationRequested)
            {
                const float delay = 1f;
                CLog.Log($"[{_hero.gameObject.name}] Closest enemy is null. Waiting {delay} sec.");
                await Task.Delay((int)(delay * 1000f), token);
                _enemy = BattleManager.GetBestTargetForAttack(_hero);
            }
            if (token.IsCancellationRequested)
                return;
            _rangeCoverCheck.Update(_hero.transform.position, false);
            await Task.Yield();
            DecideNextStep(token);
        }

        private async void DecideNextStep(CancellationToken token)
        {
            if (_enemy.IsDead)
            {
                _mainToken.Cancel();
                _mainToken = new CancellationTokenSource();
                SearchAndAttack(_mainToken.Token);
                return;
            }
            if (token.IsCancellationRequested)
                return;
            _hero.HeroView.agent.SetCellMoveCheck(OnMoveStepCallback);
            var actionCall = CheckIfShallMove(_enemy);
            // CLog.LogGreen($"[{_hero.gameObject.name}] DecideNextStep res: {actionCall}");
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
                    var shouldMove = _hero.Battle.AttackPositionCalculator.GetPositionToAttack(_hero, _enemy, out var enemyCell);
                    if (!shouldMove)
                    {
                        CLog.LogRed($"[DecideNextStep] Wierd case");
                        await Task.Yield();
                        if (token.IsCancellationRequested)
                            return;
                        DecideNextStep(token);
                    }
                    _state = AttackBehaviourState.Moving;
                    _hero.HeroView.movement.TargetCell = enemyCell;
                    StopSubProc();
                    WaitingForMovement(enemyCell, _subToken.Token);
                    return;
                case 2:
                    StopSubProc();
                    if (_state == AttackBehaviourState.Attacking)
                    {
                        StopAttack();
                    }
                    _state = AttackBehaviourState.Rotating;
                    // CLog.LogYellow($"Rotating. Enemy {_enemy.gameObject.name}. EnemyCell {_enemy.HeroView.agent.CurrentCell.ToString()}");
                    _hero.HeroView.movement.RotateIfNecessary(_enemy.HeroView.agent.CurrentCell, _subToken.Token, OnRotated);
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
            CLog.Log($"[{_hero.gameObject.name}] Stop Attack");
        }
        
        private async Task BeginAttackAndCheckIfDead(CancellationToken token)
        {
            _hero.HeroView.AttackManager.BeginAttack(_enemy.transform, _enemy.HeroView.DamageReceiver);
            var startEnemyCell = _enemy.HeroView.agent.CurrentCell;
            while (!token.IsCancellationRequested)
            {
                if (_enemy.IsDead)
                {
                    DecideNextStep(_mainToken.Token);
                    return;
                }
                var currentEnemyCell = _enemy.HeroView.agent.CurrentCell;
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
        
       
        // private void Draw(Vector3 worldPos, Vector3 enemyPos, Vector2Int cell)
        // {
        //     var p1 = worldPos;
        //     var p2 = p1 + Vector3.up * 3;
        //     
        //     var enp1 = enemyPos;
        //     var enp2 = enp1 + Vector3.up * 3;
        //
        //     var cp1 = _hero.HeroView.agent.Map.GetWorldFromCell(cell);
        //     var cp2 = cp1 + Vector3.up * 3;
        //     var time = 2f;
        //     
        //     Debug.DrawLine(p1, p2, Color.green, time);
        //     Debug.DrawLine(enp1, enp2, Color.red, time);
        //     Debug.DrawLine(cp1, cp2, Color.magenta, time);
        // }
    }
}