using RobotCastle.Core;
using RobotCastle.Data;
using SleepDev;

namespace RobotCastle.Merging
{
    public class MergeMaxLevelCheck : IMergeMaxLevelCheck
    {
        public bool CanUpgradeFurther(CoreItemData itemData)
        {
            var db = ServiceLocator.Get<ViewDataBase>();
            var max = db.GetMaxMergeLevel(itemData.id);
            if (max < 0)
            {
                CLog.LogError($"Error! Cannot determine max level for merge id: {itemData.id}");
                return false;
            }
            return (itemData.level + 1) < max;
        }
    }
}