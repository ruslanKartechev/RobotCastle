﻿using System;
using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class Battle
    {
        public Battle()
        {
            _enemies = new();
            _players = new ();
            _playersAlive = new();
            _enemies = new ();
            playerTeam = new BattleTeam();
            enemyTeam = new BattleTeam();
            _enemiesAlive = new ();
            _playersAlive = new ();
        }

        public static Battle GetDefault()
        {
            return new Battle()
            {
                troopSize = 3,
                roundIndex = 0,
                playerHealthPoints = 3,
            };
        }

        
        public Action<int> WinCallback { get; set; }
        
        public BattleState State { get; set; }
        
        public int roundIndex = 0;
        public int troopSize = HeroesConstants.PlayerTroopsStart;
        public int playerHealthPoints = HeroesConstants.PlayerHealthStart;
        public BattleTeam playerTeam;
        public BattleTeam enemyTeam;
        public bool isCompleted => _completed;

        public List<IHeroController> enemiesAlive => _enemiesAlive;

        public List<IHeroController> playersAlive => _playersAlive;
        
        public AttackPositionCalculator AttackPositionCalculator { get; set; } = new();
        
        public BattleRewardCalculator RewardCalculator { get; set; }
        public List<IBattleHeroKilledListener> HeroKilledListener { get; private set; } = new(10);
        public List<IBattleHeroKilledListener> EnemyKilledListener { get; private set; } = new(10);


        public BattleTeam GetTeam(int num) => num == 0 ? playerTeam : enemyTeam;
        public BattleTeam GetEnemyTeam(int num) => num == 0 ? enemyTeam : playerTeam;
        
        public void Reset()
        {
            _completed = false;
            State = BattleState.NotStarted;
        }

        public void OnKilled(IHeroController hero)
        {
            switch (hero.TeamNum)
            {
                case 0:
                    foreach (var listener in HeroKilledListener)
                        listener.OnKilled(hero);
                    _playersAlive.Remove(hero);
                    if (!_completed && _playersAlive.Count == 0)
                    {
                        _completed = true;
                        WinCallback?.Invoke(1); // enemies won
                    }
                    break;
                default:
                    foreach (var listener in EnemyKilledListener)
                        listener.OnKilled(hero);
                    // RewardCalculator.AddRewardForKill(hero);
                    _enemiesAlive.Remove(hero);
                    if (!_completed && _enemiesAlive.Count == 0)
                    {
                        _completed = true;
                        WinCallback?.Invoke(0); // player won
                    }
                    break;
            }
        }

        public void AddEnemy(IHeroController hero)
        {
            _enemies.Add(hero);
            _enemiesAlive.Add(hero);
        }

        public void RemovePlayer(IHeroController hero)
        {
            _players.Remove(hero);
            _playersAlive.Remove(hero);
        }
        
        public void AddPlayer(IHeroController hero)
        {
            _players.Add(hero);
            _playersAlive.Add(hero);
        }

        public List<IHeroController> Enemies
        {
            get => _enemies;
            set
            {
                _enemies = value;
                _enemiesAlive.Clear();
                _enemiesAlive.AddRange(_enemies);
                enemyTeam.ourUnits = _enemiesAlive;
                playerTeam.enemyUnits = _enemiesAlive;
            }
        }

        public void UpdateEnemiesAliveList()
        {
            _enemiesAlive.Clear();
            _enemiesAlive.AddRange(_enemies);
        }
        
        public List<IHeroController> PlayerUnits
        {
            get => _players;
            set
            {
                _players = value;
                _playersAlive.Clear();
                _playersAlive.AddRange(_players);
                playerTeam.ourUnits = _playersAlive;
                enemyTeam.enemyUnits = _playersAlive;
            }
        }

        public string GetMainStateAsStr()
        {
            return $"TroopSize: {troopSize}. Stage {roundIndex}. PlayerHealth: {playerHealthPoints}. Player Count: {_players.Count}. Enemies count: {_enemies.Count}";
        }

        public string GetPlayerAsStr()
        {
            var msg = $"Players Total: {_players.Count}. Alive {_playersAlive.Count}\n";
            var num = 1;
            foreach (var hero in _players)
            {
                msg += $"{num} {hero.Components.stats.HeroId}\n";
            }
            return msg;
        }

        public string GetEnemiesAsStr()
        {
            var msg = $"Enemies total: {_enemies.Count}. Alive {_enemiesAlive.Count}\n";
            var num = 1;
            foreach (var hero in _enemies)
            {
                msg += $"{num} {hero.Components.stats.HeroId}\n";
            }
            return msg;
        }
        
        private bool _completed;
        private List<IHeroController> _enemies; // is not changed. All units set at startup
        private List<IHeroController> _players; // is not changed. All units set at startup
        
        private List<IHeroController> _enemiesAlive = new(20);
        private List<IHeroController> _playersAlive = new(20);
        
    }
}