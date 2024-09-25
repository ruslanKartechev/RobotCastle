using System.Collections.Generic;
using RobotCastle.Battling;
using RobotCastle.Data;

namespace RobotCastle.Merging
{
    public interface IItemsChoiceListener
    {
        void ConfirmChosenItems(List<HeroItemData> items, List<int> chosen, List<int> left);
    }
}