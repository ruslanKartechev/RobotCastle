namespace RobotCastle.Battling
{
    public class ManaResetFull : IManaReset
    {
        public void Reset(HeroComponents heroView)
        {
            heroView.stats.ManaCurrent.SetBaseAndCurrent(heroView.stats.ManaMax.Val);
        }
    }
}