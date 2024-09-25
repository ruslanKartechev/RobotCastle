using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.Battling
{
    public interface IHeroItemsContainer
    {
        int MaxCount { get; }
        int ItemsCount { get; }
        List<HeroItemData> Items { get; }
        void ReplaceWithMergedItem(int indexAt, HeroItemData newItem);
        void AddNewItem(HeroItemData newItem);
        void UpdateItems(List<HeroItemData> items);
        void SetItems(List<HeroItemData> items);
    }
}