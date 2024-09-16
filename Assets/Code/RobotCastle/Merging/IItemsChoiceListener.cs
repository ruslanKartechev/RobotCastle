using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.Merging
{
    public interface IItemsChoiceListener
    {
        void ConfirmChosenItems(List<CoreItemData> chosen, List<CoreItemData> left);
    }
}