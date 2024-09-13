using RobotCastle.Core;
using RobotCastle.Merging;
using RobotCastle.UI;
using UnityEngine;

namespace RobotCastle.Battling
{
    public class BookItemView : MonoBehaviour, IItemView, IHeroItemDescriptionProvider, IUpgradeItem
    {
        [SerializeField] private int _maxItemLevelIndexToUpgrade;

        public int MaxItemLevelIndexToUpgrade
        {
            get => _maxItemLevelIndexToUpgrade;
            set => _maxItemLevelIndexToUpgrade = value;
        }
        
        public ItemData itemData { get; set; }
        
        public Transform Transform => transform;
        
        public void InitView(ItemData data)
        {
            itemData = data;
        }

        public void UpdateViewToData(ItemData data = null)
        {
            if(data != null)
                itemData = data;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPicked() { }

        public void OnPut() { }

        public void OnDroppedBack() { }

        public void OnMerged() { }

        public string GetIdForUI() => UIConstants.DescriptionItemLong;

        public GameObject GetGameObject() => gameObject;

        public Vector3 WorldPosition => transform.position;

        public int Lvl => itemData.core.level;
        
        public Sprite GetItemIcon()
        {
            return ServiceLocator.Get<ViewDataBase>().GetUnitItemSpriteAtLevel(itemData.core.id, Lvl);
        }

        public DescriptionInfo GetInfo()
        {
            var id = itemData.core.id;
            var fullId = $"{id}_lvl_{Lvl}";
            return ServiceLocator.Get<DescriptionsDataBase>().GetDescription(fullId);
        }

        public EItemDescriptionMode Mode => EItemDescriptionMode.DescriptionOnly;
        
        public bool CanUpgrade(CoreItemData item)
        {
            return item.level <= _maxItemLevelIndexToUpgrade;
        }
    }
}