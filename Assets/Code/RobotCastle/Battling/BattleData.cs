using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class BattleData
    {
        public int troopSize = 3;
        public int stageIndex = 0;
        public int playerHealthPoints = 3;
        public BattleTeam playerTeam;
        public BattleTeam enemyTeam;

        private List<HeroController> _enemies;
        private List<HeroController> _playerUnits;

        public List<HeroController> Enemies
        {
            get => _enemies;
            set
            {
                _enemies = value;
                enemyTeam.ourTeam = value;
                playerTeam.otherTeam = value;
            }
        }
        
        public List<HeroController> PlayerUnits
        {
            get => _playerUnits;
            set
            {
                _playerUnits = value;
                playerTeam.ourTeam = value;
                enemyTeam.otherTeam = value;
            }
        }


        public BattleData()
        {
            playerTeam = new BattleTeam();
            enemyTeam = new BattleTeam();
        }

        public static BattleData GetDefault()
        {
            return new BattleData()
            {
                troopSize = 3,
                stageIndex = 0,
                playerHealthPoints = 3,
            };
        }

        public string GetAsStr()
        {
            return $"TroopSize: {troopSize}. Stage {stageIndex}. PlayerHealth: {playerHealthPoints}";
        }
    }

    [System.Serializable]
    public class BattleTeam
    {
        public List<HeroController> ourTeam = new();
        public List<HeroController> otherTeam = new();
        
    }
}