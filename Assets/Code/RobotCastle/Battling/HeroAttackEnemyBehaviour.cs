using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SleepDev;

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
            // CLog.LogRed($"{name} Attack Activate");
            if (_rangeCoverCheck == null)
                _rangeCoverCheck = new HeroRangeCoverCheck(_hero);
            _hero.Battle.AttackPositionCalculator.AddUnit(_hero.Components.state);
            _mainToken?.Cancel();
            _mainToken = new CancellationTokenSource();
            _hero.Components.stats.CheckManaFull();
            SearchAndAttack(_mainToken.Token);
        }

        public void Stop()
        {
            // CLog.LogRed($"{name} Attack behaviour stopped");
            movement.Stop();
            _isActivated = false;
            _logicStep = EAttackLogicStep.Waiting;
            _mainToken?.Cancel();
            _subToken?.Cancel();
            _hero.Components.attackManager.Stop();
            _hero.Battle.AttackPositionCalculator.RemoveUnit(_hero.Components.state);
            myState.SetTargetCellToSelf();
            _enemiesInRange.Clear();
        }
        
        
        private IHeroController _hero;
        private CancellationTokenSource _mainToken;
        private CancellationTokenSource _subToken;
        private HeroRangeCoverCheck _rangeCoverCheck;
        private EAttackLogicStep _logicStep;
        private bool _isActivated;
        private List<IHeroController> _enemiesInRange = new (10);

        private string name => _hero.Components.gameObject.name;

        private HeroStateData myState => _hero.Components.state;
        private HeroStateData enemyState => myState.attackData.CurrentEnemy.Components.state;
        private HeroMovementManager movement => _hero.Components.movement;
        
        private IHeroController enemy
        {
            get => _hero.Components.state.attackData.CurrentEnemy;
            set => _hero.Components.state.attackData.CurrentEnemy = value;
        }

        private async void SearchAndAttack(CancellationToken token)
        {
            if (!_isActivated || token.IsCancellationRequested)
                return;
            _enemiesInRange.Clear();
            myState.attackData.IsMovingForDuel = false;
            myState.SetTargetCellToSelf();
            _rangeCoverCheck.Update(_hero.Components.transform.position, true);
            _subToken?.Cancel();
            _subToken = new CancellationTokenSource();
            var enemies = BattleManager.GetBestTargetForAttack(_hero, enemy);
            while ((enemies == null || enemies.Count == 0) && !token.IsCancellationRequested)
            {
                const int waitTimMs = 500;
                CLog.Log($"[{name}] Closest enemy is null. Waiting {waitTimMs} ms.");
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
            if (token.IsCancellationRequested) return;
            
            if (TryStartAttacking())
                return;
            
            if (token.IsCancellationRequested) return;
            
            if (_logicStep == EAttackLogicStep.Attacking)
                _hero.Components.attackManager.Stop();
            _logicStep = EAttackLogicStep.Moving;

            StopSubProc();
            var targets= BattleManager.GetBestTargetForAttack(_hero, enemy);
            while(targets.Count == 0 && !token.IsCancellationRequested)
            {
                targets = BattleManager.GetBestTargetForAttack(_hero, enemy);
                await Task.Delay(250, token);
            }
            if (token.IsCancellationRequested) return;
            
            if(_logicStep == EAttackLogicStep.Attacking)
                _hero.Components.attackManager.Stop();
            _logicStep = EAttackLogicStep.Moving;
            movement.MoveToEnemy(targets[0], MoveStepCallback, EndCallback);
        }

        private void MoveStepCallback()
        {
            TryStartAttacking();
        }

        private void EndCallback()
        {
            if (!TryStartAttacking())
            {
                DecideNextStep(_mainToken.Token);
            }
        }

        private bool TryStartAttacking()
        {
            if (CheckAttackCondition(out var targetEnemy))
            {
                StopSubProc();
                movement.Stop();
                _logicStep = EAttackLogicStep.Attacking;
                enemy = targetEnemy;
                // CLog.LogGreen($"[{_hero.Components.gameObject.name}] Starting to attack: {targetEnemy.Components.gameObject.name}");
                movement.RotateIfNecessary(enemy.Components.transform, _subToken.Token);
                BeginAttackAndCheckIfDead(targetEnemy, _subToken.Token);
                return true;
            }
            return false;
        }

        
        private bool CheckAttackCondition(out IHeroController targetEnemy)
        {
            _rangeCoverCheck.Update(myState.currentCell, true);
            var enemies = _hero.Battle.GetTeam(_hero.TeamNum).enemyUnits;
            _enemiesInRange.Clear();
            foreach (var tempEnemy in enemies)
            {
                if (tempEnemy.IsDead || tempEnemy.Components.state.isOutOfMap)
                    continue;
                if (_rangeCoverCheck.IsHeroWithinRange(tempEnemy))
                    _enemiesInRange.Add(tempEnemy);
            }
            if (_enemiesInRange.Count == 0)
            {
                targetEnemy = null;
                return false;
            }
            targetEnemy = BattleManager.GetBestTarget(_hero, _enemiesInRange);
            return true;        
        }

        private void StopSubProc()
        {
            _subToken?.Cancel();
            _subToken = new CancellationTokenSource();
        }

        private async Task BeginAttackAndCheckIfDead(IHeroController targetEnemy, CancellationToken token)
        {
            _hero.Components.attackManager.BeginAttack(targetEnemy.Components.damageReceiver);
            var startEnemyCell = targetEnemy.Components.state.currentCell;
            while (!token.IsCancellationRequested)
            {
                if (targetEnemy.IsDead || targetEnemy.Components.state.isOutOfMap)
                {
                    DecideNextStep(_mainToken.Token);
                    return;
                }
                var currentEnemyCell = targetEnemy.Components.state.currentCell;
                if (startEnemyCell != currentEnemyCell)
                {
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