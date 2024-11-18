using System.Collections.Generic;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using RobotCastle.Saving;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.MainMenu
{
    public class BarracksHeroesPool : MonoBehaviour
    {
        private readonly Dictionary<string, IItemView> _heroesMap = new(30);

        public IItemView GetHero(string id)
        {
            return _heroesMap[id];
        }
        
        public void SpawnAll()
        {
            var heroes = ServiceLocator.Get<IDataSaver>().GetData<SavePlayerHeroes>().heroSaves;
            var items = new List<CoreItemData>(heroes.Count);
            foreach (var hh in heroes)
                items.Add(new CoreItemData(0, hh.id, ItemsIds.TypeHeroes));
            var factory = ServiceLocator.Get<IMergeItemsFactory>();
            var views = factory.SpawnItems(items);
            for (var i = 0; i < views.Count; i++)
            {
                var view = views[i];
                var itemData = new ItemData(level: 0, id: heroes[i].id, type: ItemsIds.TypeHeroes);
                view.itemData = itemData;
                _heroesMap.Add(heroes[i].id, view);
                var go = view.Transform.gameObject;
                go.SetActive(false);
                if (go.TryGetComponent<IItemDescriptionProvider>(out var descr))
                {
                    if(descr is UnityEngine.Component comp)
                        Destroy(comp);
                }
            }
            views.Clear();
        }
    }
    
}