using System;
using System.Threading.Tasks;
using RobotCastle.Core;
using UnityEngine;

namespace RobotCastle.Shop
{
    public class ShopPurchaseInGameMoney : MonoBehaviour, IShopPurchaseMaker
    {
        [SerializeField] private bool _hardMoney;
        [SerializeField] private int _cost;
        

        public void TryPurchase(Action<EPurchaseResult> callback)
        {
            var gm = ServiceLocator.Get<GameMoney>();
            if (_hardMoney)
            {
                var owned = gm.globalHardMoney.Val;
                if (owned < _cost)
                {
                    callback?.Invoke(EPurchaseResult.NotEnoughMoney);
                    return;
                }
                gm.globalHardMoney.AddValue(-_cost);
            }
            else
            {
                var owned = gm.globalMoney.Val;
                if (owned < _cost)
                {
                    callback?.Invoke(EPurchaseResult.NotEnoughMoney);
                    return;
                }
                gm.globalMoney.AddValue(-_cost);
            }            
            callback?.Invoke(EPurchaseResult.Success);
        }
    }
}