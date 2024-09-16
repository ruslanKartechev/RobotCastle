using System.Collections.Generic;
using RobotCastle.Data;
using RobotCastle.Merging;
using SleepDev;

namespace RobotCastle.Battling
{
    public interface IBattleUnitsFactory
    {
        bool SpawnRandomHero(WeightedList<CoreItemData> ids);
        bool SpawnHeroOrItem(CoreItemData coreData, out IItemView itemView);

        bool SpawnHeroWithItems(CoreItemData coreData, List<CoreItemData> items, out IItemView itemView);
    }
}