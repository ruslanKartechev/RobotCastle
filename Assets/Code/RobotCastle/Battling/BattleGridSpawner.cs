using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.UI;
using SleepDev;
using UnityEngine;

namespace RobotCastle.Battling
{
    [DefaultExecutionOrder(5)]
    public class BattleGridSpawner : MonoBehaviour, IBattleGridSpawner
    {
        public bool SpawnRandomHero(WeightedList<CoreItemData> ids)
        {
            var coreData = ids.Random();
            return SpawnHero(coreData);
        }

        public bool SpawnHero(CoreItemData coreData)
        {
            var manager = ServiceLocator.Get<MergeManager>();
            if (manager.SpawnHero(coreData))
                return true;
            CLog.Log($"[BattleGridSpawner] No available cell!");
            var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            ui.ShowNotEnoughSpace();
            return false;
        }

        public bool SpawnRandomHero(WeightedList<string> ids)
        {
            var id = ids.Random();
            return SpawnHero(id);
        }

        public bool SpawnHero(string id)
        {
            var manager = ServiceLocator.Get<MergeManager>();
            if (manager.SpawnHero(id))
                return true;
            CLog.Log($"[BattleGridSpawner] No available cell!");
            var ui = ServiceLocator.Get<IUIManager>().Show<MergeInfoUI>(UIConstants.UIMergeInfo, () => { });
            ui.ShowNotEnoughSpace();
            return false;
        }
        
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

    }

}