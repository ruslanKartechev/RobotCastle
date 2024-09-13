using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public interface IUpgradeItem
    {
        bool CanUpgrade(CoreItemData item);
    }
}