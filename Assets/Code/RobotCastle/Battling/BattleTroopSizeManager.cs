using RobotCastle.Core;
using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public class BattleTroopSizeManager : ITroopSizeManager
    {
        private int _purchasesCount;
        private int _startSize = 3;
        private int _addedPrice = 5;
        private int _price = 5;

        private Battle _battle;

        public BattleTroopSizeManager(Battle battle)
        {
            _battle = battle;
        }
        
        public bool CanPurchase()
        {
            var money = ServiceLocator.Get<GameMoney>().Money;
            return money >= _price;
        }

        public int GetCost()
        {
            return _price;
        }

        public int TryPurchase()
        {
            var money = ServiceLocator.Get<GameMoney>().Money;
            if (money < _price)
                return 1;
            money -= _price;
            _price += _addedPrice;
            _purchasesCount++;
            _battle.troopSize++;
            var gridController = ServiceLocator.Get<IGridSectionsController>();
            gridController.SetMaxCount(_battle.troopSize);
            ServiceLocator.Get<GameMoney>().Money = money;
            return 0;
        }
    }
}