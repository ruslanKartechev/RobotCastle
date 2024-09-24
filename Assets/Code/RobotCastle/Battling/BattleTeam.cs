using System.Collections.Generic;

namespace RobotCastle.Battling
{
    [System.Serializable]
    public class BattleTeam
    {
        public List<IHeroController> ourUnits = new();
        public List<IHeroController> enemyUnits = new();
    }
}