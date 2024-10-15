using System.Collections.Generic;

namespace RobotCastle.Merging
{
    public interface IMergeItemsContainer
    {
        void RemoveItem(IItemView view);
        void AddNewItem(IItemView view);
        List<IItemView> allItems { get; }

    }
}