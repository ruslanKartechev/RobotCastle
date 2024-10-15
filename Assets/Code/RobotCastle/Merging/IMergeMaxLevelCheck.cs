using RobotCastle.Data;

namespace RobotCastle.Merging
{
    public interface IMergeMaxLevelCheck
    {
        bool CanUpgradeFurther(CoreItemData itemData);
    }
}