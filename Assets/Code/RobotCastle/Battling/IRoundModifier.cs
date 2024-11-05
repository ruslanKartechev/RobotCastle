using RobotCastle.InvasionMode;

namespace RobotCastle.Battling
{
    public interface IRoundModifier
    {
        EnemyPackPreset ModifyPreset(EnemyPackPreset preset, RoundType roundType);
        void OnRoundSet(BattleManager battleManager);
        void OnRoundCompleted(BattleManager battleManager);
        
    }
}