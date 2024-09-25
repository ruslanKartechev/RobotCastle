namespace RobotCastle.Battling
{
    public class ManaResetFull : IManaReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.Stats.ManaCurrent.SetBaseAndCurrent(heroView.Stats.ManaMax.Val);
        }
    }
}