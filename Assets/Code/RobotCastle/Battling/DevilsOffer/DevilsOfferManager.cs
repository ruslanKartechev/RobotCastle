using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;

namespace RobotCastle.Battling.DevilsOffer
{
    public class DevilsOfferManager 
    {
        public DevilsOfferManager(DevilsOfferConfig config, EnemiesManager enemiesManager, IGridView playerGrid,
            IGridSectionsController sectionsController, ITroopSizeManager troops, BattleManager battleManager, CastleHealthView healthView)
        {
            this.config = config;
            this.enemiesManager = enemiesManager;
            this.sectionsController = sectionsController;
            this.troops = troops;
            this.battleManager = battleManager;
            this.healthView = healthView;
            this.playerGrid = playerGrid;
        }

        public DevilsOfferConfig config;
        public EnemiesManager enemiesManager;
        public IGridView playerGrid;
        public IGridSectionsController sectionsController;
        public ITroopSizeManager troops;
        
        public CastleHealthView healthView;
        public BattleManager battleManager;
        private int _index;
        private DevilsOfferData _currentOption;
        private System.Action _callback;

        public void MakeNextOffer(System.Action callback)
        {
            if (_index >= config.optionsPerTier.Count)
            {
                CLog.LogError($"[{nameof(DevilsOfferManager)}] _index >= config.optionsPerTier.Count");
                return;
            }
            _callback = callback;
            var index = _index;
            _index++;
            _currentOption = new (config.optionsPerTier[index].options[0]);
            if (_currentOption.penaltyType == EDevilsPenaltyType.CastleDurability
                && battleManager.battle.playerHealthPoints <= 1)
            {
                CLog.Log($"Random type is -CastleHealth, but health already == 1. Changing type to additional enemy forces");
                _currentOption.penaltyType = EDevilsPenaltyType.AdditionalEnemyForces;
                _currentOption.penaltyValue = .25f;
            }
            
            var ui = ServiceLocator.Get<IUIManager>().Show<DevilsOfferUI>(UIConstants.UIDevilsOffer, () => {});
            ui.Show(_currentOption, ResultCallback);
            
        }
        
        private void ResultCallback(bool playerChoice)
        {
            CLog.Log($"[{nameof(DevilsOfferManager)}] Player response: {playerChoice}");
            if (playerChoice)
            {
                var data = _currentOption.reward;
                AddReward(data);
                AddPenalty(_currentOption.penaltyType, _currentOption.penaltyValue);
            }
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
        
        private void AddReward(CoreItemData data)
        {
            switch (data.type)
            {
                case MergeConstants.TypeItems:
                    var factory = ServiceLocator.Get<IHeroesAndItemsFactory>();
                    factory.SpawnHeroOrItem(new SpawnMergeItemArgs(_currentOption.reward), playerGrid, sectionsController, out var item);
                    break;
                case MergeConstants.TypeBonus:
                    switch (data.id)
                    {
                        case "bonus_troops":
                            CLog.Log($"Adding troops size by 1");
                            troops.ExtendBy(1);
                            break;
                        case "bonus_money":
                            CLog.Log($"Adding money +{data.level}");
                            var gm = ServiceLocator.Get<GameMoney>();
                            gm.AddMoney(data.level);
                            break;
                    }
                    break;
            }
        }

        
    }
}