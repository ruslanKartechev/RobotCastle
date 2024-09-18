using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(10)]
    public class BattleManager : MonoBehaviour
    {
        [SerializeField] private bool _activateEnemies = true;
        [SerializeField] private bool _activatePlayers = true;
        [SerializeField] private string _preset;
        private Battle _battle;
        
        public Battle battle => _battle;

        public static HeroController GetBestTargetForAttack(HeroController hero)
        {
            var map = hero.HeroView.agent.Map;
            var myPos = map.GetCellPositionFromWorld(hero.transform.position);
            var cellsMask = hero.HeroView.Stats.Range.GetCellsMask();
            var coveredCells = new List<Vector2Int>(cellsMask.Count);
            foreach (var val in cellsMask)
                coveredCells.Add(myPos + val);
            var enemies = hero.Battle.GetTeam(hero.TeamNum).enemyUnits;
            var closest = (HeroController)null;
            var minD2 = int.MaxValue;
            var d2 = minD2;
            foreach (var otherHero in enemies)
            {
                var enemyPos = map.GetCellPositionFromWorld(otherHero.transform.position);
                d2 = (enemyPos - myPos).sqrMagnitude;
                if (d2 < minD2)
                {
                    minD2 = d2;
                    closest = otherHero;
                }
                if (coveredCells.Contains(enemyPos))
                {
                    CLog.Log($"Found hero at: {enemyPos.ToString()}");
                    return otherHero;
                }
            }
            return closest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hero">Your hero</param>
        /// <param name="enemy">Enemy hero to get to</param>
        /// <returns>True if should me. False if already on the cell. out targetPosition - position to move to</returns>
        public static bool GetPositionToAttack(HeroController hero, HeroController enemy, out Vector2Int targetCell)
        {
            var map = hero.HeroView.agent.Map;
            var myPos = map.GetCellPositionFromWorld(hero.transform.position);
            var cellsMask = hero.HeroView.Stats.Range.GetCellsMask();
            var coveredCells = new List<Vector2Int>(cellsMask.Count);
            foreach (var val in cellsMask)
                coveredCells.Add(myPos + val);
            var enemyPos = map.GetCellPositionFromWorld(enemy.transform.position);
            if (coveredCells.Contains(enemyPos))
            {
                targetCell = enemyPos;
                return false; // don't move, just attack
            }
            targetCell = hero.HeroView.Stats.Range.GetClosestCell(myPos, enemyPos);
            return true;
        }

        public static Vector2Int GetCellPos(Vector3 worldPos)
        {
            return new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.z));
        }


        public void Init()
        {
            CLog.Log($"[{nameof(BattleManager)}] Init Created new battle data");
            _battle = Battle.GetDefault();
            ServiceLocator.Bind<Battle>(_battle);
        }

        public bool BeginBattle()
        {
            _battle.PlayerUnits = ServiceLocator.Get<IGridSectionsController>()
                .GetItemsInActiveArea<HeroController>(_ => true);
            if (_battle.PlayerUnits.Count == 0)
            {
                CLog.LogWhite($"No Player Units on active area!");
                return false;
            }
            ServiceLocator.Get<BattleCamera>().MoveToBattlePoint();
            _battle.WinCallback = OnTeamWin;
            foreach (var hero in _battle.Enemies)
            {
                hero.TeamNum = 1;
                hero.Battle = _battle;
                hero.PrepareForBattle();
                hero.UpdateMap(true);
            }
            foreach (var hero in _battle.PlayerUnits)
            {
                hero.TeamNum = 0;
                hero.Battle = _battle;
                hero.PrepareForBattle();
                hero.UpdateMap(true);
            }

            if (_activatePlayers)
            {
                foreach (var unit in _battle.PlayerUnits)
                    unit.SetBehaviour(new HeroAttackEnemyBehaviour());
            }
            if (_activateEnemies)
            {
                foreach (var unit in _battle.Enemies)
                    unit.SetBehaviour(new HeroAttackEnemyBehaviour());
            }
            return true;
        }

        private void OnTeamWin(int teamNum)
        {
            CLog.Log($"[{nameof(BattleManager)}] On Team win {teamNum}");
            foreach (var unit in _battle.Enemies)
            {
                if(!unit.IsDead)
                    unit.SetIdle();
            }
            foreach (var unit in _battle.PlayerUnits)
            {
                if(!unit.IsDead)
                    unit.SetIdle();
            }
            switch (teamNum)
            {
                case 0:
                    CLog.Log($"[{nameof(BattleManager)}] On Player team win");
                    break;
                case 1:
                    CLog.Log($"[{nameof(BattleManager)}] On Enemy team win");
                    break;
            }
        }

        public void NextStage()
        {
        }

        public void SetStage(int stageIndex)
        {
            _battle.stageIndex = stageIndex;
            var enemies = ServiceLocator.Get<EnemiesManager>();
            enemies.SpawnPreset(_preset);
            _battle.Enemies = enemies.Enemies;
            ServiceLocator.Get<MergeManager>().AllowInput(true);
            // do something for merge reset
            _battle.PlayerUnits = ServiceLocator.Get<IGridSectionsController>()
                .GetItemsInActiveArea<HeroController>(_ => true);
        }

        public void PrintState()
        {
            if (_battle == null)
            {
                CLog.Log("_battleData == null");
                return;
            }
            var msg = _battle.GetMainStateAsStr();
            CLog.LogWhite(msg);
        }
        
        public void PrintPlayerHeroes()
        {
            if (_battle == null)
            {
                CLog.Log("_battleData == null");
                return;
            }
            var msg = _battle.GetPlayerAsStr();
            CLog.LogWhite(msg);
        }

        public void PrintEnemyHeroes()
        {
            if (_battle == null)
            {
                CLog.Log("_battleData == null");
                return;
            }
            var msg = _battle.GetEnemiesAsStr();
            CLog.LogWhite(msg);
        }
        
        private void OnEnable()
        {
            ServiceLocator.Bind<BattleManager>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<BattleManager>();
        }
        
    }
}