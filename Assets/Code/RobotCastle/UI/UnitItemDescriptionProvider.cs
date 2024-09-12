using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.UI
{
    public class UnitItemDescriptionProvider : MonoBehaviour, IItemDescriptionProvider
    {
        [SerializeField] private UnitItemMergeView _mergeView;
        
        public int Lvl => _mergeView.itemData.core.level;
        
        public DescriptionInfo GetInfo()
        {
            var id = _mergeView.itemData.core.id;
            var fullId = $"{id}_lvl_{Lvl}";
            return ServiceLocator.Get<DescriptionsDataBase>().GetDescription(fullId);
        }

        public Sprite GetItemIcon()
        {
            return ServiceLocator.Get<ViewDataBase>().GetUnitItemSpriteAtLevel(_mergeView.itemData.core.id, Lvl);
        }
        
        public string GetIdForUI()
        {
            if (_mergeView.itemData.core.level < 3)
                return "ui_item_description_short";
            else
                return "ui_item_description_long";
        }
        
        public GameObject GetGameObject() => gameObject;
        
        public Vector3 WorldPosition => transform.position;
    }
}