using RobotCastle.Merging;
using SleepDev;

namespace RobotCastle.Battling
{
    public interface IBattleGridSpawner
    {
        bool SpawnRandomHero(WeightedList<CoreItemData> ids);
        bool SpawnHero(CoreItemData coreData);
        bool SpawnRandomHero(WeightedList<string> ids);
        bool SpawnHero(string id);
    }
}