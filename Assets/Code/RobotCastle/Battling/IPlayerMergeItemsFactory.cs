
using RobotCastle.Core;

namespace RobotCastle.Battling
{
    public interface IPlayerMergeItemsFactory : IModifiable<IPlayerItemSpawnModifier>
    {
        int NextCost { get; }
        void TryPurchaseItem(bool promptUser = true);
        RobotCastle.Merging.IItemView SpawnHeroOrItem(SpawnMergeItemArgs args);
    }
}