using RobotCastle.Core;
using RobotCastle.InvasionMode;
using SleepDev;

namespace RobotCastle.Battling
{
    public class RoundModifierEnemyForcesIncrease : IRoundModifier
    {
        private int _maxRounds;
        private int _passedRounds;
        private float _additionalEnemiesPercent;
        
        public RoundModifierEnemyForcesIncrease(float enemiesPercent, int maxRounds)
        {
            _maxRounds = maxRounds;
            _additionalEnemiesPercent = enemiesPercent;
        }

        public EnemyPackPreset ModifyPreset(EnemyPackPreset preset, RoundType roundType) => preset;

        public void OnRoundSet(BattleManager battleManager)
        {
            if (battleManager.isRoundBoss)
                return;
            CLog.Log($"[EnemyForcesIncreaseRoundModifier] Applying !");
            var enManager = ServiceLocator.Get<EnemiesManager>();
            enManager.IncreaseEnemyForcesBy(_additionalEnemiesPercent);
            battleManager.battle.UpdateEnemiesAliveList();
        }

        public void OnRoundCompleted(BattleManager battleManager)
        {
            _passedRounds++;
            if (_passedRounds >= _maxRounds)
            {
                battleManager.RemoveRoundModifier(this);
            }
        }

    }
}