using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev.UIElements;
using UnityEngine;

namespace RobotCastle.UI
{
    public class PurchaseNewHeroButton : MyButton
    {
        [SerializeField] private CostViewUI _costView;
        private IPlayerMergeItemsFactory _factory;

        private void Start()
        {
            var gm = ServiceLocator.Get<GameMoney>();
            _factory = ServiceLocator.Get<IPlayerMergeItemsFactory>();
            _costView.InitWithCost(gm.levelMoney, _factory.NextCost);
            _costView.DoReact(true);   
            AddMainCallback(Purchase);
        }

        private void Purchase()
        {
            _factory.TryPurchaseItem();
        }

    }
}