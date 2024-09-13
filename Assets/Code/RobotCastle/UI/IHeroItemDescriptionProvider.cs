using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.UI
{
    public interface IHeroItemDescriptionProvider : IItemDescriptionProvider
    {
        Sprite GetItemIcon();
        DescriptionInfo GetInfo();
        EItemDescriptionMode Mode { get; }
    }
}