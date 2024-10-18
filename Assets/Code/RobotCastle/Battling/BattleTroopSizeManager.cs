using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev.Data;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BattleTroopSizeManager : ITroopSizeManager
    {

        public BattleTroopSizeManager(Battle battle, IGridSectionsController sectionsController, ParticleSystem particle)
        {
            _particle = particle;
            _battle = battle;
            _sectionsController = sectionsController;
            _sectionsController.SetMaxCount(battle.troopSize);
            _costReact = new ReactiveInt(startCost);
        }

        public ReactiveInt NextCost => _costReact;

        public bool CanPurchase()
        {
            var money = ServiceLocator.Get<GameMoney>().levelMoney.Val;
            return money >= _costReact.Val;
        }

        public int GetCost() => _costReact.Val;

        /// <summary>
        /// </summary>
        /// <returns>0 if success. 1 if not enough money</returns>
        public int TryPurchase()
        {
            var money = ServiceLocator.Get<GameMoney>().levelMoney.Val;
            var cost = _costReact.Val;
            if (money < cost)
                return 1;
            money -= cost;
            cost += costAddedPerPurchase;
            _purchasesCount++;
            _battle.troopSize++;
            _costReact.SetValueOnEvent(cost);
            _sectionsController.SetMaxCount(_battle.troopSize);
            ServiceLocator.Get<GameMoney>().levelMoney.SetValue(money);
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

        private const int startCost = 10;
        private const int costAddedPerPurchase = 5;
        
        private int _purchasesCount;
        private ParticleSystem _particle;
        private IGridSectionsController _sectionsController;
        private Battle _battle;
        private ReactiveInt _costReact;

    }
}