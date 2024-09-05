using System.Collections.Generic;

namespace RobotCastle.Merging
{
    public interface IItemsChoiceListener
    {
        void ConfirmChosenItems(List<CoreItemData> chosen, List<CoreItemData> left);
    }
}