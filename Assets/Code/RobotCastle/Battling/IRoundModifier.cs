namespace RobotCastle.Battling
{
    public interface IRoundModifier
    {
        void OnRoundSet(BattleManager battleManager);
        void OnRoundCompleted(BattleManager battleManager);
        
    }
}