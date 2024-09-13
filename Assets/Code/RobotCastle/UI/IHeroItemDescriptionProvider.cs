using RobotCastle.Data;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.UI
{
    public interface IHeroItemDescriptionProvider : IItemDescriptionProvider
    {
        CoreItemData CoreData { get; }
        Sprite GetItemIcon();
        DescriptionInfo GetInfo();
        EItemDescriptionMode Mode { get; }
    }
}