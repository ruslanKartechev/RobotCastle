using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class BattleTeam
    {
        public List<HeroController> ourUnits = new();
        public List<HeroController> enemyUnits = new();
        
    }
}