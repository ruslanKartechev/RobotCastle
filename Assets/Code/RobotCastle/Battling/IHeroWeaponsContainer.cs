using System.Collections.Generic;
using RobotCastle.Data;

namespace RobotCastle.Battling
{
    public interface IHeroWeaponsContainer
    {
        int MaxCount { get; }
        int ItemsCount { get; }
        List<HeroWeaponData> Items { get; }
        void ReplaceWithMergedItem(int indexAt, HeroWeaponData newItem);
        void AddNewItem(HeroWeaponData newItem);
        void UpdateItems(List<HeroWeaponData> items);
        void SetItems(List<HeroWeaponData> items);
        void SetEmpty();
        void AddAllModifiersToHero(HeroComponents hero);
    }
}