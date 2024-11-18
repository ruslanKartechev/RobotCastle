using System.Collections.Generic;
using RobotCastle.Data;
using RobotCastle.Merging;

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
            if (view.itemData.core.type == ItemsIds.TypeHeroes)
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
        
    }
}