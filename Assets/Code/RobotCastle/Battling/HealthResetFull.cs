namespace RobotCastle.Battling
{
    public class HealthResetFull : IHealthReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.Stats.HealthCurrent.SetBaseAndCurrent(heroView.Stats.HealthMax.Val);
        }
    }
}