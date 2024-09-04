using System.Collections.Generic;
using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public interface IUnitsItemsContainer
    {
        int MaxCount { get; }
        int ItemsCount { get; }
        List<CoreItemData> Items { get; }
        void ReplaceWithMergedItem(int indexAt, CoreItemData newItem);
        void AddNewItem(CoreItemData newItem);
        void UpdateView();
        void SetItems(List<CoreItemData> items);
    }
}