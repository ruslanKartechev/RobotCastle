using System.Collections.Generic;
using RobotCastle.Merging;
using SleepDev;

namespace RobotCastle.Battling
{
    public interface IBattleGridSpawner
    {
        bool SpawnRandomHero(WeightedList<CoreItemData> ids);
        bool SpawnHero(CoreItemData coreData, out IItemView itemView);

        bool SpawnHeroWithItems(CoreItemData coreData, List<CoreItemData> items, out IItemView itemView);
    }
}