using RobotCastle.Core;
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
        
        
        public void OnRoundSet(BattleManager battleManager)
        {
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
    
    
    public class RoundModifierEnemiesTierUp : IRoundModifier
    {
        private int _maxRounds;
        private int _passedRounds;
        private int _tierBonus;
        
        public RoundModifierEnemiesTierUp(int tierBonus, int maxRounds)
        {
            _maxRounds = maxRounds;
            _tierBonus = tierBonus;
        }
        
        
        public void OnRoundSet(BattleManager battleManager)
        {
            CLog.Log($"[EnemyForcesIncreaseRoundModifier] Applying !");
            var enManager = ServiceLocator.Get<EnemiesManager>();
            enManager.RaiseEnemiesTierAll(_tierBonus);
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