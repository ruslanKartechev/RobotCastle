
using RobotCastle.Core;
using SleepDev.Data;

namespace RobotCastle.Battling
{
    public interface IPlayerMergeItemsFactory : IModifiable<IPlayerItemSpawnModifier>
    {
        ReactiveInt NextCost { get; }
        void TryPurchaseItem(bool promptUser = true);
        RobotCastle.Merging.IItemView SpawnHeroOrItem(SpawnMergeItemArgs args);
    }
}