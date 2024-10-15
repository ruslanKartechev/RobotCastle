namespace RobotCastle.Battling
{
    public class HealthResetFull : IHealthReset
    {
        public void Reset(HeroComponents heroView)
        {
            heroView.stats.HealthCurrent.SetBaseAndCurrent(heroView.stats.HealthMax.Get());
        }
    }
}