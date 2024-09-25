namespace RobotCastle.Battling
{
    public class ManaResetZero : IManaReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.Stats.ManaCurrent.SetBaseAndCurrent(0);
        }
    }
}