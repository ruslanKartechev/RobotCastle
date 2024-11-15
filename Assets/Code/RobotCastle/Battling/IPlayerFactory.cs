using System;
using RobotCastle.Core;
using SleepDev.Data;

namespace RobotCastle.Battling
{
    public enum PurchaseHeroResult {Success, NotEnoughMoney, NotEnoughSpace, NotAllowed, Error}
    public interface IPlayerFactory : IModifiable<IPlayerItemSpawnModifier>
    {
        public event Action<PurchaseHeroResult> OnPurchase;
        
        bool PurchaseAllowed { get; set; }
        ReactiveInt NextCost { get; }
        PurchaseHeroResult TryPurchaseItem(bool promptUser = true);
        RobotCastle.Merging.IItemView SpawnHeroOrItem(SpawnArgs args);
    }
}