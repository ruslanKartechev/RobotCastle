using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Battling
{
    public class UnitItemsContainerView : MonoBehaviour
    {
        [SerializeField] private GameObject _viewBlock;
        [SerializeField] private List<Image> _icons;

        public void ShowItems(List<CoreItemData> items)
        {
            On();
            var db = ServiceLocator.Get<ViewDataBase>();
            var itemsCount = items.Count;
            for (var ind = 0; ind < itemsCount; ind++)
            {
                _icons[ind].gameObject.SetActive(true);
                _icons[ind].sprite = db.GetUnitItemSpriteAtLevel(items[ind].id, items[ind].level);
            }
            for (var i = itemsCount; i < _icons.Count; i++)
                _icons[i].gameObject.SetActive(false);
        }

        public void UpdateMergedItem(List<CoreItemData> items, int index)
        {
            On();
            var db = ServiceLocator.Get<ViewDataBase>();
            _icons[index].sprite = db.GetUnitItemSpriteAtLevel(items[index].id, items[index].level);
            
            ShowOnlyUsedGameobjects(items);
        }

        public void ShowLastAddedItem(List<CoreItemData> items)
        {
            On();
            var db = ServiceLocator.Get<ViewDataBase>();
            var lastInd = items.Count - 1;
            _icons[lastInd].sprite = db.GetUnitItemSpriteAtLevel(items[lastInd].id, items[lastInd].level);

            ShowOnlyUsedGameobjects(items);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShowOnlyUsedGameobjects(List<CoreItemData> items)
        {
            var itemsCount = items.Count;
            for (var ind = 0; ind < itemsCount; ind++)
                _icons[ind].gameObject.SetActive(true);
            for (var i = itemsCount; i < _icons.Count; i++)
                _icons[i].gameObject.SetActive(false);
        }
        
        public void Off() => _viewBlock.gameObject.SetActive(false);
        
        public void On() => _viewBlock.gameObject.SetActive(true);
    }
}