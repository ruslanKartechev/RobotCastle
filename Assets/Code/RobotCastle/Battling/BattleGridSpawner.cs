using RobotCastle.Core;
using RobotCastle.Merging;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(5)]
    public class BattleGridSpawner : MonoBehaviour, IBattleGridSpawner
    {
        private void OnEnable()
        {
            ServiceLocator.Bind<IBattleGridSpawner>(this);
            ServiceLocator.Bind<BattleGridSpawner>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unbind<IBattleGridSpawner>();
            ServiceLocator.Unbind<BattleGridSpawner>();
        }

        public bool SpawnRandomHero(WeightedList<CoreItemData> ids)
        {
            var coreData = ids.Random();
            return SpawnHero(coreData);
        }

        public bool SpawnHero(CoreItemData coreData)
        {
            var controller = ServiceLocator.Get<MergeController>();
            if (controller.GetFreeCellForNewHero(out var view))
            {
                var spawner = ServiceLocator.Get<IGridItemsSpawner>();
                spawner.SpawnItemOnCell(view, new ItemData(coreData.level, coreData.id, coreData.type));
                return true;
            }
            else
            {
                CLog.Log($"No available cell!");
                return true;
            }
        }
        

        public bool SpawnRandomHero(WeightedList<string> ids)
        {
            var id = ids.Random();
            return SpawnHero(id);
        }

        public bool SpawnHero(string id)
        {
            var controller = ServiceLocator.Get<MergeController>();
            if (controller.GetFreeCellForNewHero(out var view))
            {
                var spawner = ServiceLocator.Get<IGridItemsSpawner>();
                spawner.SpawnItemOnCell(view, id);
                return true;
            }
            else
            {
                CLog.Log($"No available cell!");
                return true;
            }
        }

        public bool CanSpawnNew()
        {
            var controller = ServiceLocator.Get<MergeController>();
            return controller.GetFreeCellForNewHero(out var cellView);
        }
    }

}