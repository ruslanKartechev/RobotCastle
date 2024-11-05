using RobotCastle.Core;
using RobotCastle.InvasionMode;
using RobotCastle.Merging;
using SleepDev;

namespace RobotCastle.Battling
{
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

        public EnemyPackPreset ModifyPreset(EnemyPackPreset preset, RoundType roundType)
        {
            CLog.Log($"[EnemyForcesIncreaseRoundModifier] ModifyPreset !");
            foreach (var en in preset.enemies)
            {
                var lvl = en.enemy.level;
                if ((lvl + 1) < MergeConstants.HeroMaxMergeLvl)
                {
                    lvl++;
                    en.enemy.level = lvl;
                }
            }
            return preset;
        }

        public void OnRoundSet(BattleManager battleManager)
        { }

        public void OnRoundCompleted(BattleManager battleManager)
        {
            _passedRounds++;
            if (_passedRounds >= _maxRounds)
            {
                battleManager.RemoveRoundModifier(this);
            }
        }

        public void ApplyNow(BattleManager battleManager)
        {
            if (battleManager.isRoundBoss)
                return;
            CLog.Log($"[EnemyForcesIncreaseRoundModifier] ApplyNow !");
            var enManager = ServiceLocator.Get<EnemiesManager>();
            enManager.RaiseEnemiesTierAll(_tierBonus);
        }
    }
}