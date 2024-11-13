using RobotCastle.Core;
using SleepDev;

namespace RobotCastle.Battling
{
    public class BattleRewardCalculator
    {
        public BattleRewardCalculator() {}

        public BattleRewardCalculator(int perKill, int perStageCompletion)
        {
            this._perKill = perKill;
            this._perStageCompletion = perStageCompletion;
        }

        public int RewardPerKill
        {
            get => _perKill;
            set => _perKill = value;
        }

        public int RewardPerStageCompletion
        {
            get => _perStageCompletion;
            set => _perStageCompletion = value;
        }
        
        private int _perKill = 0;
        private int _perStageCompletion = 10;
        
        public void AddRewardForKill(IHeroController hero)
        {
            // CLog.Log($"Adding reward: {_perKill}");
            if (_perKill > 0)
            {
                var gm = ServiceLocator.Get<GameMoney>();
                gm.AddMoney(_perKill);              
            }
        }

        public void AddRewardForStage(bool win)
        {
            var gm = ServiceLocator.Get<GameMoney>();
            gm.AddMoney(_perStageCompletion);
        }
        
    }
}