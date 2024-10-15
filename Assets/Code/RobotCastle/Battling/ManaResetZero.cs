namespace RobotCastle.Battling
{
    public class ManaResetZero : IManaReset
    {
        public void Reset(HeroComponents heroView)
        {
            heroView.stats.ManaCurrent.SetBaseAndCurrent(0);
        }
    }
}