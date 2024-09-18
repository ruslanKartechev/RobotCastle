using System.Collections.Generic;
using RobotCastle.Data;
using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public interface IHeroesAndUnitsFactory
    {
        bool SpawnHeroOrItem(SpawnMergeItemArgs args, out IItemView itemView);

        bool SpawnHeroWithItems(SpawnMergeItemArgs args, List<CoreItemData> items, out IItemView itemView);
    }
}