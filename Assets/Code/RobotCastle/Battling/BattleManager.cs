using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(10)]
    public partial class BattleManager : MonoBehaviour
    {
        [SerializeField] private bool _activateEnemies = true;
        [SerializeField] private bool _activatePlayers = true;
        private Battle _battle;
        private List<InvasionRoundData> _roundData;
        private BattleRewardCalculator _rewardCalculator;

        public BattleRewardCalculator BattleRewardCalculator => _rewardCalculator;
        public Battle battle => _battle;
        
        public IBattleEndProcessor endProcessor { get; set; }
        
        public IBattleStartedProcessor startProcessor { get; set; }
        

        public Battle Init(List<InvasionRoundData> roundData)
        {
            _roundData = roundData;
            CLog.Log($"[{nameof(BattleManager)}] Init Created new battle data");
            _battle = Battle.GetDefault();
            ServiceLocator.Bind<Battle>(_battle);
            _battle.State = BattleState.NotStarted;
            _rewardCalculator = _battle.RewardCalculator = new BattleRewardCalculator(3,5);
            return _battle;
        }

        public bool CanStart()
        {
            _battle.PlayerUnits = ServiceLocator.Get<IGridSectionsController>().GetItemsInActiveArea<IHeroController>(_ => true);
            if (_battle.PlayerUnits.Count == 0)
            {
                _battle.State = BattleState.NotStarted;
                CLog.LogWhite($"No Player Units on active area!");
                return false;
            }
            return true;
        }

        public void SetGoingState() => _battle.State = BattleState.Going;

        public bool BeginBattle()
        {
            SetupRewardForCurrentRound();
            _battle.PlayerUnits = ServiceLocator.Get<IGridSectionsController>().GetItemsInActiveArea<IHeroController>(_ => true);
            if (_battle.PlayerUnits.Count == 0)
            {
                _battle.State = BattleState.NotStarted;
                CLog.LogWhite($"No Player Units on active area!");
                return false;
            }
            _battle.State = BattleState.Going;
            _battle.WinCallback = OnTeamWin;
            var map = ServiceLocator.Get<Bomber.IMap>();
            foreach (var hero in _battle.Enemies)
            {
                hero.TeamNum = 1;
                hero.Battle = _battle;
                hero.View.agent.UpdateMap(map);
            }
            foreach (var hero in _battle.PlayerUnits)
            {
                hero.TeamNum = 0;
                hero.Battle = _battle;
                hero.View.agent.UpdateMap(map);
            }
            PrepareForBattle(_battle.PlayerUnits);
            PrepareForBattle(_battle.Enemies);
            
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
            _battle.Reset();
            _battle.stageIndex++;
            SetStage(_battle.stageIndex);
        }

        public void ResetStage()
        {
            _battle.Reset();
            SetStage(_battle.stageIndex);
        }

        public void SetStage(int stageIndex)
        {
            if (_roundData.Count <= stageIndex)
            {
                CLog.LogError($"[{nameof(BattleManager)}] Preset index out of range!");
                return;
            }
            _battle.stageIndex = stageIndex;
            foreach (var en in _battle.Enemies)
                Destroy(en.View.gameObject);
            _battle.Enemies.Clear();
            _battle.enemiesAlive.Clear();
            var preset = _roundData[stageIndex].enemyPreset;
            var enemyManager = ServiceLocator.Get<EnemiesManager>();
            enemyManager.SpawnPreset(preset);
            _battle.Enemies = enemyManager.Enemies;
            // do something for merge reset
            _battle.PlayerUnits = ServiceLocator.Get<IGridSectionsController>()
                .GetItemsInActiveArea<IHeroController>(_ => true);
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

        public void RecollectPlayerUnits()
        {
            var mergeManager = ServiceLocator.Get<MergeManager>();
            foreach (var playerUnit in _battle.PlayerUnits)
                ResetHeroAfterBattle(playerUnit);
            mergeManager.ResetAllItemsPositions();
        }
        
        public void SetupRewardForCurrentRound()
        {
            _rewardCalculator.RewardPerStageCompletion = _roundData[_battle.stageIndex].reward;
        }
        
        
        private void OnTeamWin(int teamNum)
        {
            CLog.Log($"[{nameof(BattleManager)}] On Team win {teamNum}");
            foreach (var unit in _battle.enemiesAlive)
                unit.SetBehaviour(new HeroIdleBehaviour());
            foreach (var unit in _battle.playersAlive)
                unit.SetBehaviour(new HeroIdleBehaviour());
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