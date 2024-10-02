namespace RobotCastle.Core
{
    public class GameState
    {
        public static EGameMode Mode { get; set; }
        
        public enum EGameMode
        {
            MainMenu,
            InvasionBattle
        }
    }
}