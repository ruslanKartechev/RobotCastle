using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using UnityEngine;
using UnityEngine.UI;

namespace RobotCastle.Battling
{
    public class HeroWeaponsContainerUI : MonoBehaviour
    {
        [SerializeField] private GameObject _viewBlock;
        [SerializeField] private List<Image> _icons;

        public void Empty()
        {
            for (var i = 0; i < _icons.Count; i++)
                _icons[i].gameObject.SetActive(false);   
            Off();
        }
        
        public void ShowItems(List<HeroWeaponData> items)
        {
            On();
            var db = ServiceLocator.Get<ViewDataBase>();
            var itemsCount = items.Count < _icons.Count ? items.Count : _icons.Count;
            for (var ind = 0; ind < itemsCount; ind++)
            {
                _icons[ind].gameObject.SetActive(true);
                _icons[ind].sprite = db.GetUnitItemSpriteAtLevel(items[ind].core.id, items[ind].core.level);
            }
            for (var i = itemsCount; i < _icons.Count; i++)
                _icons[i].gameObject.SetActive(false);
        }

        public void UpdateMergedItem(List<HeroWeaponData> items, int index)
        {
            On();
            var db = ServiceLocator.Get<ViewDataBase>();
            _icons[index].sprite = db.GetUnitItemSpriteAtLevel(items[index].core.id, items[index].core.level);
            
            ShowOnlyUsedGameobjects(items);
        }

        public void ShowLastAddedItem(List<HeroWeaponData> items)
        {
            On();
            var db = ServiceLocator.Get<ViewDataBase>();
            var lastInd = items.Count - 1;
            _icons[lastInd].sprite = db.GetUnitItemSpriteAtLevel(items[lastInd].core.id, items[lastInd].core.level);

            ShowOnlyUsedGameobjects(items);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShowOnlyUsedGameobjects(List<HeroWeaponData> items)
        {
            var itemsCount = items.Count;
            for (var ind = 0; ind < itemsCount; ind++)
                _icons[ind].gameObject.SetActive(true);
            for (var i = itemsCount; i < _icons.Count; i++)
                _icons[i].gameObject.SetActive(false);
        }

        public void Animate()
        {
            
        }
        
        public void Off() => _viewBlock.gameObject.SetActive(false);
        
        public void On() => _viewBlock.gameObject.SetActive(true);
    }
}