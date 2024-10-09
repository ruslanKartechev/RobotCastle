using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleTroopSizeManager : ITroopSizeManager
    {
        private int _purchasesCount;
        private int _startSize = 3;
        private int _addedPrice = 5;
        private int _price = 5;
        private ParticleSystem _particle;

        private Battle _battle;

        public BattleTroopSizeManager(Battle battle, ParticleSystem particle)
        {
            _particle = particle;
            _battle = battle;
        }
        
        public bool CanPurchase()
        {
            var money = ServiceLocator.Get<GameMoney>().levelMoney;
            return money >= _price;
        }

        public int GetCost()
        {
            return _price;
        }

        /// <summary>
        /// </summary>
        /// <returns>0 if success. 1 if not enough money</returns>
        public int TryPurchase()
        {
            var money = ServiceLocator.Get<GameMoney>().levelMoney;
            if (money < _price)
                return 1;
            money -= _price;
            _price += _addedPrice;
            _purchasesCount++;
            _battle.troopSize++;
            var gridController = ServiceLocator.Get<IGridSectionsController>();
            gridController.SetMaxCount(_battle.troopSize);
            ServiceLocator.Get<GameMoney>().levelMoney = money;
            _particle.Play();
            return 0;
        }

        public void ExtendBy(int count)
        {
            _battle.troopSize++;
            var gridController = ServiceLocator.Get<IGridSectionsController>();
            gridController.SetMaxCount(_battle.troopSize);
            _particle.Play();

        }
    }
}