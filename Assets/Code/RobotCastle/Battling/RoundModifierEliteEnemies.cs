using RobotCastle.InvasionMode;
using SleepDev;

namespace RobotCastle.Battling
{
    public class RoundModifierEliteEnemies : IRoundModifier
    {

        public RoundModifierEliteEnemies(int count)
        {
            _additionalCount = count;
        }
        
        public int AdditionalCount => _additionalCount;
    
        public EnemyPackPreset ModifyPreset(EnemyPackPreset preset, RoundType roundType)
        {
            if (roundType != RoundType.EliteEnemy)
                return preset;
            CLog.Log($"[RoundModifierEliteEnemies] ModifyPreset !");
            var defaultEnemies = preset.enemies.FindAll(t => t.unitType == EUnitType.Default);
            if (defaultEnemies.Count == 0)
                return preset;
            for (var i = 0; i < defaultEnemies.Count && i < _additionalCount; i++)
            {
                defaultEnemies[i].unitType = EUnitType.Elite;
                defaultEnemies[i].canDropItems = true;
            }
            return preset;
        }

        public void OnRoundSet(BattleManager battleManager)
        { }

        public void OnRoundCompleted(BattleManager battleManager)
        { }
        
        private int _additionalCount;

    }
}