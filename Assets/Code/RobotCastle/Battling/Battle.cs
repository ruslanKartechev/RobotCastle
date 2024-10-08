using System;
using System.Collections.Generic;
using SleepDev;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class Battle
    {
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

        private bool _completed;
        private List<IHeroController> _enemies; // is not changed. All units set at startup
        private List<IHeroController> _players; // is not changed. All units set at startup
        
        private List<IHeroController> _enemiesAlive;
        private List<IHeroController> _playersAlive;
        
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
                    _playersAlive.Remove(hero);
                    if (!_completed && _playersAlive.Count == 0)
                    {
                        _completed = true;
                        WinCallback?.Invoke(1); // enemies won
                    }
                    break;
                default:
                    RewardCalculator.AddRewardForKill(hero);
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
                _enemiesAlive = new List<IHeroController>(value);
                enemyTeam.ourUnits = _enemiesAlive;
                playerTeam.enemyUnits = _enemiesAlive;
            }
        }
        
        public List<IHeroController> PlayerUnits
        {
            get => _players;
            set
            {
                _players = value;
                _playersAlive = new List<IHeroController>(value);
                playerTeam.ourUnits = _playersAlive;
                enemyTeam.enemyUnits = _playersAlive;
            }
        }

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
                msg += $"{num} {hero.View.stats.HeroId}\n";
            }
            return msg;
        }

        public string GetEnemiesAsStr()
        {
            var msg = $"Enemies total: {_enemies.Count}. Alive {_enemiesAlive.Count}\n";
            var num = 1;
            foreach (var hero in _enemies)
            {
                msg += $"{num} {hero.View.stats.HeroId}\n";
            }
            return msg;
        }
    }
}