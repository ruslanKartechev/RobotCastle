using System;
using RobotCastle.Battling;
using RobotCastle.Core;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.UI
{
    public class UnitItemDescriptionProvider : MonoBehaviour, IHeroItemDescriptionProvider
    {
        private IItemView _mergeView;

        private void OnEnable()
        {
            _mergeView = gameObject.GetComponent<IItemView>();
        }

        public int Lvl => _mergeView.itemData.core.level;
        
        public DescriptionInfo GetInfo()
        {
            var id = _mergeView.itemData.core.id;
            var fullId = $"{id}_lvl_{Lvl}";
            return ServiceLocator.Get<DescriptionsDataBase>().GetDescription(fullId);
        }

        public EItemDescriptionMode Mode => EItemDescriptionMode.Modifier;

        public Sprite GetItemIcon()
        {
            return ServiceLocator.Get<ViewDataBase>().GetUnitItemSpriteAtLevel(_mergeView.itemData.core.id, Lvl);
        }
        
        public string GetIdForUI()
        {
            if (_mergeView.itemData.core.level < 3)
                return UIConstants.DescriptionItemShort;
            else
                return UIConstants.DescriptionItemLong;
        }
        
        public GameObject GetGameObject() => gameObject;
        
        public Vector3 WorldPosition => transform.position;
    }
}