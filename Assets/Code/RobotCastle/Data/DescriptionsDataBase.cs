using System.Collections.Generic;
using RobotCastle.Merging;
using SleepDev;

namespace RobotCastle.Data
{
    [System.Serializable]
    public class DescriptionsDataBase
    {
        public Dictionary<string, DescriptionInfo> descriptions;

        public DescriptionInfo GetDescription(string id)
        {
            return descriptions[id];
        }

        public DescriptionInfo GetDescriptionByLevel(CoreItemData itemData)
        {
            var id = itemData.id;
            var fullId = $"{id}_{itemData.level}";
            return descriptions[fullId];
        }
        
        public DescriptionInfo GetDescriptionByTypeAndLevel(CoreItemData itemData)
        {
            switch (itemData.type)
            {
                case ItemsIds.TypeItem:
                    return GetDescriptionByLevel(itemData);
                case ItemsIds.TypeBonus:
                    var key = $"{itemData.id}_{itemData.level}";
                    if (descriptions.ContainsKey(key) == false)
                    {
                        CLog.LogError($"[DescriptionsDb] does not contain: {key}");
                        return null;
                    } 
                    return descriptions[key];
                default:
                    return descriptions[itemData.id];
            }
        }
 
    }
}