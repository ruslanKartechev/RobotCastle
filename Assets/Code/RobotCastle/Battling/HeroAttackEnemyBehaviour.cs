using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bomber;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class HeroAttackEnemyBehaviour : IHeroBehaviour
    {
        private HeroController _hero;
        private HeroController _enemy;
        private CancellationTokenSource _token;
        private Action<IHeroBehaviour> _callback;
        private HeroRangeCoverCheck _rangeCoverCheck;
        
        public string BehaviourID => "hero_attack";
        
        public void Activate(GameObject target, Action<IHeroBehaviour> endCallback)
        {
            var hero = target.GetComponent<HeroController>();
            Activate(hero, endCallback);
        }

        public void Activate(HeroController hero, Action<IHeroBehaviour> endCallback)
        {
            _hero = hero;
            _callback = endCallback;
            if (_rangeCoverCheck == null)
                _rangeCoverCheck = new HeroRangeCoverCheck(_hero);
            AttackClosest();
        }

        public void Stop()
        {
            CLog.LogRed($"[{nameof(HeroAttackEnemyBehaviour)}] [Stop]");
            if(_token != null)
                _token.Cancel();
            _hero.HeroView.agent.SetCellMoveCheck(null);
            _hero.HeroView.AttackManager.Stop();
        }

        private void AttackClosest()
        {
            if(_token != null)
                _token.Cancel();
            CLog.LogYellow($"[{nameof(HeroAttackEnemyBehaviour)}] [{_hero.gameObject.name}] [AttackClosest]");
            _token = new CancellationTokenSource();
            SearchAndAttack(_token.Token);
        }
        
        // 1 Choose target
        // 2 go to target
        // 3 attack target
        // while going  - check position
        private async void SearchAndAttack(CancellationToken token)
        {
            _enemy = BattleManager.GetBestTargetForAttack(_hero);
            _rangeCoverCheck.Update(_hero.transform.position, true);
            while (_enemy == null && !token.IsCancellationRequested)
            {
                const float delay = 1f;
                CLog.Log($"[{_hero.gameObject.name}] Closest enemy is null. Waiting {delay} sec.");
                await Task.Delay((int)(delay * 1000f), token);
                _enemy = BattleManager.GetBestTargetForAttack(_hero);
            }
            if (token.IsCancellationRequested)
                return;
            _rangeCoverCheck.Update(_hero.transform.position, true);
            await Task.Yield();
            _hero.HeroView.agent.SetCellMoveCheck(MoveToCellCallback);
            var didMove = await MovingToChosenEnemy(token);
            if(!didMove)
                await Attacking(token);
            else
                AttackClosest();
        }

        private bool MoveToCellCallback(Vector2Int cell)
        {
            _enemy = BattleManager.GetBestTargetForAttack(_hero);
            _rangeCoverCheck.Update(_hero.HeroView.agent.CurrentCell, false);
            var isInRange = _rangeCoverCheck.IsHeroWithinRange(_enemy);
            if (isInRange)
            {
                CLog.LogRed($"[MoveToCellCallback] [{_hero.gameObject.name}] The enemy is in range. Attacking");
                _token?.Cancel();
                _token = new CancellationTokenSource();
                SearchAndAttack(_token.Token);
                return false;
            }
            return true;
        }

        private async Task Attacking(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;
            _hero.HeroView.AttackManager.BeginAttack(_enemy.transform, _enemy.HeroView.DamageReceiver);
            var enemyCell = _enemy.HeroView.agent.CurrentCell;
            while (!token.IsCancellationRequested)
            {
                if (_enemy.IsDead)
                {
                    // CLog.Log($"[{_hero.gameObject.name}] Target has died. Moving to next one");
                    _hero.HeroView.AttackManager.Stop();
                    await Task.Yield();
                    if (token.IsCancellationRequested)
                        return;
                    AttackClosest();
                    return;
                }
                var newCell = _enemy.HeroView.agent.CurrentCell;
                if (enemyCell != newCell)
                {
                    // CLog.LogRed($"[{_hero.gameObject.name}] Enemy moved to another position");
                    _hero.HeroView.AttackManager.Stop();
                    AttackClosest();
                    return;
                }
                if (token.IsCancellationRequested)
                    return;
                await Task.Yield();
            }
        }
        
        /// <summary>
        /// </summary>
        /// <returns>True if did move, false if didn't</returns>
        private async Task<bool> MovingToChosenEnemy(CancellationToken token)
        {
            var shouldMove = BattleManager.GetPositionToAttack(_hero, _enemy, out var cellPos);
            if (shouldMove)
            {
                Draw(_hero.transform.position, _enemy.transform.position, cellPos);
                await _hero.HeroView.movement.MoveToCell(cellPos, token);
                if (token.IsCancellationRequested)
                    return false;
                var state = _hero.HeroView.agent.State;
                if (state != PathfindingAgent.AgentState.Arrived)
                {
                    CLog.LogRed($"[{_enemy.gameObject.name}] Failed to arrive at the point!");
                    await Task.Yield();
                    return true;
                }
            }
            return await _hero.HeroView.movement.RotateIfNecessary(_enemy.HeroView.agent.CurrentCell, token);
        }
        
        private void Draw(Vector3 worldPos, Vector3 enemyPos, Vector2Int cell)
        {
            var p1 = worldPos;
            var p2 = p1 + Vector3.up * 3;
            
            var enp1 = enemyPos;
            var enp2 = enp1 + Vector3.up * 3;

            var cp1 = _hero.HeroView.agent.Map.GetWorldFromCell(cell);
            var cp2 = cp1 + Vector3.up * 3;
            var time = 2f;
            
            Debug.DrawLine(p1, p2, Color.green, time);
            Debug.DrawLine(enp1, enp2, Color.red, time);
            Debug.DrawLine(cp1, cp2, Color.magenta, time);
        }
    }
}