using System.Collections.Generic;
using RobotCastle.Merging;
using SleepDev;

namespace RobotCastle.Battling
{
    public class PlayerMergeItemsContainer : IMergeItemsContainer
    {
        public List<IItemView> allItems => _allItems;

        public List<IHeroController> heroes => _heroes;
        
        private readonly List<IItemView> _allItems = new (20);
        private readonly List<IHeroController> _heroes = new (20);
        

        public void AddNewItem(IItemView view)
        {
            // CLog.Log($"Added item: {view.itemData.core.id}");
            _allItems.Add(view);
            if (view.itemData.core.type == MergeConstants.TypeHeroes)
            {
                var h = view.Transform.gameObject.GetComponent<IHeroController>();
                _heroes.Add(h);
            }
        }

        public void RemoveItem(IItemView view)
        {
            // CLog.Log($"Removed item: {view.itemData.core.id}");
            _allItems.Remove(view);
        }
        
        // public void RemoveHero(IItemView view)
        // {
        //     Debug.Log($"Removed hero: {view.itemData.core.id}");
        //     var h = view.Transform.gameObject.GetComponent<IHeroController>();
        //     _allItems.Remove(view);
        //     _heroes.Remove(h);
        // }
        //
        // public void AddHero(IItemView view)
        // {
        //     Debug.Log($"Added new hero: {view.itemData.core.id}");
        //     var h = view.Transform.gameObject.GetComponent<IHeroController>();
        //     _allItems.Add(view);
        //     _heroes.Add(h);
        // }
        
    }
}