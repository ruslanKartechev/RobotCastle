using RobotCastle.Data;

namespace RobotCastle.Merging
{
    public class MergeSwapAllowedCheck : ISwapAllowedCheck
    {
        public bool IsSwapAllowed(ICellView cell1, ICellView cell2)
        {
            if (cell1.itemView.itemData.core.id == ItemsIds.ItemUpgradeBookId ||
                cell2.itemView.itemData.core.id == ItemsIds.ItemUpgradeBookId)
            {
                return false;
            }
            return true;
        }
    }
}