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

        
        [SerializeField] private bool _activateEnemies = true;
        [SerializeField] private bool _activatePlayers = true;
        private Battle _battle;
        private PresetsContainer _presetsContainer;
        
        public Battle battle => _battle;
        
        public IBattleEndProcessor endProcessor { get; set; }
        
        public IBattleStartedProcessor startProcessor { get; set; }
        

        public void Init(PresetsContainer presetsContainer)
        {
            CLog.Log($"[{nameof(BattleManager)}] Init Created new battle data");
            _presetsContainer = presetsContainer;
            _battle = Battle.GetDefault();
            ServiceLocator.Bind<Battle>(_battle);
            _battle.State = BattleState.NotStarted;
        }
        
        public bool BeginBattle()
        {
            _battle.PlayerUnits = ServiceLocator.Get<IGridSectionsController>()
                .GetItemsInActiveArea<HeroController>(_ => true);
            if (_battle.PlayerUnits.Count == 0)
            {
                _battle.State = BattleState.NotStarted;
                CLog.LogWhite($"No Player Units on active area!");
                return false;
            }
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
                foreach (var unit in _battle.playersAlive)
                    unit.SetBehaviour(new HeroAttackEnemyBehaviour());
            }
            if (_activateEnemies)
            {
                foreach (var unit in _battle.enemiesAlive)
                    unit.SetBehaviour(new HeroAttackEnemyBehaviour());
            }
            _battle.State = BattleState.Going;
            startProcessor?.OnBattleStarted(_battle);
            return true;
        }
        
        public void NextStage()
        {
            _battle.stageIndex++;
            SetStage(_battle.stageIndex);
        }

        public void ResetStage()
        {
            SetStage(_battle.stageIndex);
        }

        public void SetStage(int stageIndex)
        {
            _battle.stageIndex = stageIndex;
            foreach (var en in _battle.Enemies)
                Destroy(en.gameObject);
            _battle.Enemies.Clear();
            _battle.enemiesAlive.Clear();
            var preset = _presetsContainer.Presets[stageIndex];
            var enemyManager = ServiceLocator.Get<EnemiesManager>();
            enemyManager.SpawnPreset(preset);
            _battle.Enemies = enemyManager.Enemies;
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
                    _battle.State = BattleState.PlayerWin;
                    endProcessor?.OnBattleEnded(_battle);
                    break;
                case 1:
                    CLog.Log($"[{nameof(BattleManager)}] On Enemy team win");
                    _battle.State = BattleState.EnemyWin;
                    endProcessor?.OnBattleEnded(_battle);
                    break;
            }
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