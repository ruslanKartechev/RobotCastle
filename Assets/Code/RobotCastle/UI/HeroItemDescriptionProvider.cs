using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.Merging;
using UnityEngine;

namespace RobotCastle.UI
{
    public class HeroItemDescriptionProvider : MonoBehaviour, IHeroItemDescriptionProvider
    {
        private IItemView _mergeView;

        private void OnEnable()
        {
            _mergeView = gameObject.GetComponent<IItemView>();
        }

        public CoreItemData CoreData => _mergeView.itemData.core;
        
        public DescriptionInfo GetInfo() => ServiceLocator.Get<DescriptionsDataBase>().GetDescriptionByLevel(_mergeView.itemData.core);
        
        public Sprite GetItemIcon() => DataHelpers.GetItemIcon(_mergeView.itemData.core);

        public EItemDescriptionMode Mode => EItemDescriptionMode.Modifier;

        public string GetIdForUI()
        {
            if (_mergeView.itemData.core.level >= 3)
                return UIConstants.DescriptionItemLong;
            return UIConstants.DescriptionItemShort;
        }
        
        public GameObject GetGameObject() => gameObject;
        
        public Vector3 WorldPosition => transform.position;
    }
}