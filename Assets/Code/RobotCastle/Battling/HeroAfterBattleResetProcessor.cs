namespace RobotCastle.Battling
{
    public class HeroAfterBattleResetProcessor : IHeroAfterBattleResetProcessor
    {
        private HeroView _heroView;

        public HeroAfterBattleResetProcessor(HeroView heroView)
        {
            _heroView = heroView;
        }

        public void ResetForMerge()
        {
            _heroView.Stats.ClearDecorators();
            
            _heroView.Stats.HealthReset.Reset(_heroView);
            _heroView.Stats.ManaReset.Reset(_heroView);
            _heroView.heroUI.UpdateStatsView(_heroView);
            
            _heroView.HealthManager.SetDamageable(true);
            _heroView.animator.WriteDefaultValues();
            _heroView.heroUI.Show();
            _heroView.AttackData.Reset();
            _heroView.gameObject.SetActive(true);
        }
    }
}