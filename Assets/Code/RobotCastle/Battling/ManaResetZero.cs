namespace RobotCastle.Battling
{
    public class ManaResetZero : IManaReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.stats.ManaCurrent.SetBaseAndCurrent(0);
        }
    }
}