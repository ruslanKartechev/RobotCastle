using RobotCastle.Battling;
using RobotCastle.Core;
using SleepDev.UIElements;
using UnityEngine;

namespace RobotCastle.UI
{
    public class TroopSizePurchaseUI : MyButton
    {
        [SerializeField] private CostViewUI _costViewUI;
        private ITroopSizeManager _manager;
        private bool _didSub;

        private void Start()
        {
            _manager = ServiceLocator.Get<ITroopSizeManager>();
            var gm = ServiceLocator.Get<GameMoney>();
            _costViewUI.InitWithCost(gm.levelMoney, _manager.NextCost);
            _costViewUI.DoReact(true);
            AddMainCallback(OnBtn);
        }
        
        private void OnBtn()
        {
            _manager.TryPurchase();
        }
   
    }
}