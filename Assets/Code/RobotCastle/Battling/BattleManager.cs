using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RobotCastle.Core;
using RobotCastle.InvasionMode;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(10)]
    public partial class BattleManager : MonoBehaviour
    {
        public Battle battle => _battle;
        
        public IBattleEndProcessor endProcessor { get; set; }
        
        public IBattleStartedProcessor startProcessor { get; set; }
        public bool isRoundBoss => _battle.roundIndex == _levelData.levels.Count - 1;

        public BattleRewardCalculator rewardCalculator => _rewardCalculator;
        public RoundData currentRound => _levelData.levels[_battle.roundIndex];
        public IBattleDamageStatsCollector playerStatCollector => _playerStatsCollector;
        public IBattleDamageStatsCollector enemiesStatsCollector => _enemiesStatsCollector;
        public BattleEnemiesWeaponsDropper dropper => _dropper;
        public LevelData levelData => _levelData;
        
        
        [SerializeField] private bool _activateEnemies = true;
        [SerializeField] private bool _activatePlayers = true;
        private Battle _battle;
        private LevelData _levelData;
        private BattleRewardCalculator _rewardCalculator;
        private BattleEnemiesWeaponsDropper _dropper;
        private List<IRoundModifier> _roundModifiers = new(5);
        private BattleDamageStatsCollector _playerStatsCollector = new();
        private BattleDamageStatsCollector _enemiesStatsCollector = new();
        
        
        public Battle CreateBattle(LevelData levelData)
        {
            _levelData = levelData;
            CLog.Log($"[{nameof(BattleManager)}] Init Created new battle data");
            _battle = Battle.GetDefault();
            ServiceLocator.Bind<Battle>(_battle);
            _battle.State = BattleState.NotStarted;
            _rewardCalculator = _battle.RewardCalculator = new BattleRewardCalculator(0,0);
            _dropper = new();
            _battle.EnemyKilledListener.Add(_dropper);
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

        public EliteItemsPreset GetCurrentItemsPreset()
        {
            var itemsPresetInd = (_battle.roundIndex + 1) / 5;
            var count = _levelData.eliteItemsByLevel.Count;
            if (itemsPresetInd >= count)
            {
                CLog.Log($"ItemsPresetInd {itemsPresetInd} is >= presets count: {count}");
                itemsPresetInd = count - 1;
            }
            return _levelData.eliteItemsByLevel[itemsPresetInd];
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
            _dropper.Reset();
            _battle.State = BattleState.Going;
            _battle.WinCallback = OnTeamWin;
            var map = ServiceLocator.Get<Bomber.IMap>();
            _battle.PlayerUnits.RemoveNulls();
            _battle.Enemies.RemoveNulls();
            foreach (var hero in _battle.Enemies)
            {
                hero.TeamNum = 1;
                hero.Battle = _battle;
                hero.Components.movement.InitAgent(map);
            }
            foreach (var hero in _battle.PlayerUnits)
            {
                hero.TeamNum = 0;
                hero.Battle = _battle;
                hero.Components.movement.InitAgent(map);
            }
            PrepareForBattle(_battle.PlayerUnits);
            PrepareForBattle(_battle.Enemies);
            _playerStatsCollector.ResetForNewHeroes(_battle.PlayerUnits);
            _enemiesStatsCollector.ResetForNewHeroes(_battle.Enemies);
            if (_activatePlayers)
            {
                foreach (var hero in _battle.playersAlive)
                    hero.SetDefaultBehaviour();
            }
            if (_activateEnemies)
            {
                foreach (var hero in _battle.enemiesAlive)
                    hero.SetDefaultBehaviour();
            }
            _battle.State = BattleState.Going;
            startProcessor?.OnBattleStarted(_battle);
            return true;
        }
        
        public void AddRoundModifier(IRoundModifier modifier) => _roundModifiers.Add(modifier);

        public void RemoveRoundModifier(IRoundModifier modifier) => _roundModifiers.Remove(modifier);

        public void ClearAllModifiers() => _roundModifiers.Clear();

        public void SetNextStage()
        {
            _battle.Reset();
            _battle.roundIndex++;
        }
        
        public async Task ResetStage(CancellationToken token)
        {
            _battle.Reset();
            await SetCurrentStage(token);
        }
        
        public async Task SetCurrentStage(CancellationToken token)
        {
            try
            {
                await SetRound(_battle.roundIndex, token);
            }
            catch (System.Exception ex)
            {
                CLog.LogError($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public async Task SetRound(int roundInd, CancellationToken token)
        {
            CLog.Log($"Levels total: {_levelData.levels.Count}. RoundInd: {roundInd} ");
            _battle.roundIndex = roundInd;
            _rewardCalculator.RewardPerStageCompletion = _levelData.levels[roundInd].reward;
            var enemiesManager = ServiceLocator.Get<EnemiesManager>();
            if (_battle.Enemies.Count > 0)
            {
                enemiesManager.DestroyCurrentUnits();
                await Task.Yield();
                if (token.IsCancellationRequested) return;
            }
            
            _battle.Enemies.Clear();
            _battle.enemiesAlive.Clear();

            var roundType = _levelData.levels[roundInd].roundType;
            // CLog.Log($"Round: {roundInd + 1}, type: {roundType.ToString()}");
            
            var preset = EnemiesFactory.GetPackPreset(_levelData.levels[roundInd].enemyPreset);
            for (var i = 0; i < _roundModifiers.Count; i++)
                preset = _roundModifiers[i].ModifyPreset(preset, roundType);
            
            var itemsPreset = GetCurrentItemsPreset();
            await enemiesManager.factory.SpawnPresetEnemies(preset, itemsPreset, token);
            
            if (token.IsCancellationRequested) return;

            for (var i = 0; i < _roundModifiers.Count; i++)
                _roundModifiers[i].OnRoundSet(this);
            _battle.Enemies = enemiesManager.AllEnemies;

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
            _battle.PlayerUnits.RemoveNulls();
            _battle.playersAlive.RemoveNulls();
            foreach (var unit in _battle.PlayerUnits)
            {
                ResetHeroAfterBattle(unit);
            }
            mergeManager.ResetAllItemsPositions();
        }

        public void AddNewEnemiesDuringBattle(List<SpawnArgs> args)
        {
            if (_battle.State != BattleState.Going)
            {
                CLog.Log($"[AddNewEnemiesDuringBattle] Battle not going");
                return;
            }
            var map = ServiceLocator.Get<Bomber.IMap>();
            var enemies = ServiceLocator.Get<EnemiesManager>();
            var heroes = new List<IHeroController>(args.Count);
            foreach (var arg in args)
            {
                var hero = enemies.factory.SpawnNew(arg, 0,false);
                heroes.Add(hero);
                var pos = map.GetWorldFromCell(arg.preferredCoordinated);
                hero.Components.transform.position = pos;
            }
            
            foreach (var hero in heroes)
            {
                hero.TeamNum = 1;
                hero.Battle = _battle;
                hero.Components.movement.InitAgent(map);
            }
            
            _battle.Enemies.AddRange(heroes);
            _battle.enemiesAlive.AddRange(heroes);
            _enemiesStatsCollector.AddHeroes(heroes);
            PrepareForBattle(heroes);
            foreach (var hero in heroes)
                hero.SetBehaviour(new HeroAttackEnemyBehaviour());
        }
        
        private void SetupRewardForCurrentRound()
        {
            _rewardCalculator.RewardPerStageCompletion = _levelData.levels[_battle.roundIndex].reward;
        }
        
        private void OnTeamWin(int teamNum)
        {
            CLog.Log($"[{nameof(BattleManager)}] On Team win {teamNum}");
            foreach (var unit in _battle.enemiesAlive)
                unit.SetBehaviour(new HeroIdleBehaviour());
            foreach (var unit in _battle.playersAlive)
                unit.SetBehaviour(new HeroIdleBehaviour());
            
            for (var i = _roundModifiers.Count - 1; i >= 0; i--)
                _roundModifiers[i].OnRoundCompleted(this);
            _dropper.RevealAll();
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