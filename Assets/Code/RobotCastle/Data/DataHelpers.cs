using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.Data
{
    public static class DataHelpers
    {
        public static DescriptionInfo GetItemDescription(CoreItemData itemData)
        {
            var id = itemData.id;
            var fullId = $"{id}_lvl_{itemData.level}";
            return ServiceLocator.Get<DescriptionsDataBase>().GetDescription(fullId);
        }

        public static Sprite GetItemIcon(CoreItemData itemData)
        {
            return ServiceLocator.Get<ViewDataBase>().GetUnitItemSpriteAtLevel(itemData.id, itemData.level);
        }
    }
}