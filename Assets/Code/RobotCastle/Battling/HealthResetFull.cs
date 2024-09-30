namespace RobotCastle.Battling
{
    public class HealthResetFull : IHealthReset
    {
        public void Reset(HeroView heroView)
        {
            heroView.stats.HealthCurrent.SetBaseAndCurrent(heroView.stats.HealthMax.Val);
        }
    }
}