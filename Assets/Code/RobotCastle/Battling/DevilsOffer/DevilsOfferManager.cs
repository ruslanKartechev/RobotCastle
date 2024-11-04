using System.Collections.Generic;
using System.Threading;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling.DevilsOffer
{
    public class DevilsOfferManager 
    {
        public DevilsOfferManager(DevilsOfferConfig config, BattleManager battleManager, CastleHealthView healthView)
        {
            this.config = config;
            this.battleManager = battleManager;
            this.healthView = healthView;
        }

        public DevilsOfferConfig config;
        public CastleHealthView healthView;
        public BattleManager battleManager;
        private DevilsOfferData _currentOption;
        private System.Action _callback;
        private CancellationTokenSource _token;
        private int _offerTier;

        public void MakeNextOffer(System.Action callback)
        {
            if (_offerTier >= config.optionsPerTier.Count)
            {
                CLog.LogError($"[{nameof(DevilsOfferManager)}] _index >= config.optionsPerTier.Count");
                return;
            }
            _callback = callback;
            _token?.Cancel();
            _token = new CancellationTokenSource();
            
            var tier = _offerTier;
            _offerTier++;
            var options = new List<DevilsOfferData>(config.optionsPerTier[tier].options);
            var health = battleManager.battle.playerHealthPoints;
            if (health >= HeroesConstants.PlayerHealthStart)
            {
                options.RemoveAll(t => t.reward.id == "restore_health");
            }
            else if (health <= 1)
            {
                options.RemoveAll(t => t.penaltyType == EDevilsPenaltyType.CastleDurability);
            }
            if (options.Count == 0)
            {
                CLog.LogError($"[{nameof(DevilsOfferManager)}] Options count == 0 after filtering");
            }
            _currentOption = new (options.Random());
            var ui = ServiceLocator.Get<IUIManager>().Show<DevilsOfferUI>(UIConstants.UIDevilsOffer, () => {});
            ui.Show(_currentOption, tier, ResultCallback);
        }
        
        private void ResultCallback(bool playerChoice)
        {
            CLog.Log($"[{nameof(DevilsOfferManager)}] Player response: {playerChoice}");
            if (playerChoice)
            {
                ShowPenaltyAndRewards(_token.Token);
            }
            else
            {
                _callback?.Invoke();
            }
        }

        private async void ShowPenaltyAndRewards(CancellationToken token)
        {
            const float waitTime = 1.3f;
            var cam = ServiceLocator.Get<BattleCamera>();
            var moveToMergePoint = false;
            switch (_currentOption.penaltyType)
            {
                case EDevilsPenaltyType.AdditionalEnemyForces or EDevilsPenaltyType.HigherEnemyTier:
                    moveToMergePoint = true;
                    cam.SetBattlePoint();
                    break;
                case EDevilsPenaltyType.CastleDurability:
                    cam.SetMergePoint();
                    break;
            }
            AddPenalty(_currentOption.penaltyType, _currentOption.penaltyValue);

            await HeroesManager.WaitGameTime(waitTime, token);
            if (token.IsCancellationRequested) return;
            if (moveToMergePoint)
            {
                cam.Stop();
                await cam.MovingAndScalingToMerge(token);
                if (token.IsCancellationRequested) return;
            }
            
            var coreData = _currentOption.reward;
            HeroesManager.AddRewardOrBonus(coreData);
            
            await HeroesManager.WaitGameTime(waitTime, token);
            if (token.IsCancellationRequested) return;

            _callback?.Invoke();
        }
        
        

        private void AddPenalty(EDevilsPenaltyType type, float val)
        {
            switch (type)
            {
                case EDevilsPenaltyType.CastleDurability:
                    var minusVal = (int)val;
                    var health = battleManager.battle.playerHealthPoints;
                    health -= minusVal;
                    battleManager.battle.playerHealthPoints = health;
                    healthView.MinusHealth(battleManager.battle.playerHealthPoints);
                    break;
                case EDevilsPenaltyType.AdditionalEnemyForces:
                    var modifier = new RoundModifierEnemyForcesIncrease(val, 1);
                    battleManager.AddRoundModifier(modifier);
                    modifier.OnRoundSet(battleManager);
                    break;
                case EDevilsPenaltyType.HigherEnemyTier:
                    var modifier2 = new RoundModifierEnemiesTierUp((int)val, 1);
                    battleManager.AddRoundModifier(modifier2);
                    modifier2.OnRoundSet(battleManager);
                    break;
            }
        }
        
        
    }
}